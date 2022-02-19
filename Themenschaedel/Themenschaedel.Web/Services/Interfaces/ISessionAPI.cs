using System.Threading.Tasks;
using Themenschaedel.Shared.Models.Response;

namespace Themenschaedel.Web.Services.Interfaces
{
    public interface ISessionAPI
    {
        void DeleteSession(string token);
        Task<LoginResponseExtended> RefreshToken();
        Task<UserResponse> GetCurrentUserData();
        Task Logout();
    }
}