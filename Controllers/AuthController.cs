using AuthApp2.Models;
using AuthApp2.Repositories;
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
        private readonly IUserRepository _userRepository;
        public static List<RefreshToken> _refreshTokens = new();

        public AuthController(
            ITokenService tokenService, IUserRepository userRepository)
        {
            _tokenService = tokenService;
            _userRepository = userRepository;
        }

        [HttpPost("login")]
        public IActionResult Login(LoginRequest request)
        {
            // Demo user validation
            var user = _userRepository.GetUser(request.Username, request.Password);
            if (user == null)
                return Unauthorized();

            var accessToken = _tokenService.CreateAccessToken(
                user.UserId,
                user.UserName,
                user.Role
            );

            var (refreshToken, refreshTokenHash) = _tokenService.CreateRefreshToken();

            _refreshTokens.Add(new RefreshToken
            {
                TokenHash = refreshTokenHash,
                UserId = user.UserId,
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


            var user = _userRepository.GetUserByUserId(storedToken.UserId);
            if (user == null)
            {
                storedToken.IsRevoked = true;
                return Unauthorized("User not found");
            }
            var userId = user.UserName;
            var role = user.Role;

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

        [Authorize]
        [HttpGet("debug")]
        public IActionResult Debug()
        {
            return Ok(User.Claims.Select(c => new { c.Type, c.Value }));
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
