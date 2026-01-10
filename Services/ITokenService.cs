using AuthApp2.Models;

namespace AuthApp2.Services
{
    public interface ITokenService
    {
        string CreateAccessToken(string userId, string name, string role);
        (string rawToken, string hash) CreateRefreshToken();
    }
}
