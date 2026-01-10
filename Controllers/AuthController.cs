using AuthApp2.Models;
using AuthApp2.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using System.Text;

namespace AuthApp2.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ITokenService _tokenService;
        public static List<RefreshToken> _refreshTokens = new();

        public AuthController(
            ITokenService tokenService)
        {
            _tokenService = tokenService;
        }

        [HttpPost("login")]
        public IActionResult Login(LoginRequest request)
        {
            // Demo user validation
            if (request.Username != "User123" || request.Password != "Password123!")
                return Unauthorized();

            var userId = "User123";
            var role = "Admin";

            var accessToken = _tokenService.CreateAccessToken(
                userId,
                request.Username,
                role
            );

            var (refreshToken, refreshTokenHash) = _tokenService.CreateRefreshToken();

            _refreshTokens.Add(new RefreshToken
            {
                TokenHash = refreshTokenHash,
                UserId = userId,
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddDays(7)
            });

            return Ok(new { accessToken, refreshToken });
        }

        [HttpPost("refresh")]
        public IActionResult Refresh(RefreshRequest request)
        {
            var incomingHash = Hash(request.RefreshToken);

            var storedToken = _refreshTokens
                .SingleOrDefault(t =>
                    t.TokenHash == incomingHash &&
                    !t.IsRevoked &&
                    t.ExpiresAt > DateTime.UtcNow
                );

            if (storedToken == null)
                return Unauthorized("Invalid refresh token");

            // Rotate
            storedToken.IsRevoked = true;

            var (newRefreshToken, newHash) = _tokenService.CreateRefreshToken();

            storedToken.ReplacedByToken= newHash;

            _refreshTokens.Add(new RefreshToken
            {
                TokenHash = newHash,
                UserId = storedToken.UserId,
                ExpiresAt = DateTime.UtcNow.AddDays(30),
                CreatedAt = DateTime.UtcNow
            });
            
            var userId = "User123";
            var role = "Admin";

            var newAccessToken = _tokenService.CreateAccessToken(
                storedToken.UserId,
                userId,
                role
            );

            return Ok(new
            {
                accessToken = newAccessToken,
                refreshToken = newRefreshToken
            });
        }

        [Authorize]
        [HttpPost("logout")]
        public IActionResult Logout(RefreshRequest request)
        {
            var hash = Hash(request.RefreshToken);

            var token = _refreshTokens.SingleOrDefault(t => t.TokenHash == hash);

            if (token != null)
                token.IsRevoked = true;

            return Ok();
        }

        private static string Hash(string token)
        {
            using var sha256 = SHA256.Create();
            return Convert.ToBase64String(
                sha256.ComputeHash(Encoding.UTF8.GetBytes(token))
            );
        }
    }
}
