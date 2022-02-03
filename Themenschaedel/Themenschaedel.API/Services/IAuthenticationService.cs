using Themenschaedel.Shared.Models;
using Themenschaedel.Shared.Models.Request;

namespace Themenschaedel.API.Services
{
    public interface IAuthenticationService
    {
        public Task<bool> RegisterAsync(UserRegistration user);
        public Task<TokenExtended> LoginAsync(string username, string password);
        public Task<bool> LogoutAsync(string token);
        public Task LogoutAllAsync(string token);
        public User GetUser(string token);
        public Task<bool> VerifyEmailAsync(string verificationId);
        public Task<Token> RefreshTokenAsync(RefreshTokenRequest refreshTokenRequest);
        public bool IsTokenValid(string token);
        public bool IsTokenValid(HttpRequest request);
        public string GetValidToken(HttpRequest request);
        public Task<User> GetUserFromValidToken(HttpRequest request);
        public Task<bool> CheckIfUserHasElivatedPermission(HttpRequest request);
    }
}
