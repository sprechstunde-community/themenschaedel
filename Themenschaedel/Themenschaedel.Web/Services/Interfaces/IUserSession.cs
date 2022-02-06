using System.Threading.Tasks;
using Themenschaedel.Shared.Models;
using Themenschaedel.Shared.Models.Response;
using Themenschaedel.Web.Pages;
using Settings = Themenschaedel.Shared.Models.Settings;

namespace Themenschaedel.Web.Services.Interfaces
{
    public interface IUserSession
    {
        Task SetAuthenticationTokenAsync(LoginResponse authenticationToken, bool keepLoggedIn);
        bool IsLoggedIn();
        Task<bool> IsLoggedInAsync();
        Task<UserResponse> GetCurrentlyLoggedInUser();
        Task Logout();
        Task<bool> RecheckLoginAndClearIfInvalid();
        Task<LoginResponse> GetToken();
        Task<Settings> GetSettings();
        Task SetSettings(Settings settings);
    }
}