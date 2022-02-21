using System;
using System.Threading.Tasks;
using Themenschaedel.Shared.Models;
using Themenschaedel.Shared.Models.Response;
using Themenschaedel.Web.Pages;
using Settings = Themenschaedel.Shared.Models.Settings;

namespace Themenschaedel.Web.Services.Interfaces
{
    public interface IUserSession
    {
        Task SetAuthenticationTokenAsync(LoginResponse authenticationToken, LoginDuration keepLoggedIn);
        bool IsLoggedIn();
        Task<bool> IsLoggedInAsync();
        Task<UserResponse> GetCurrentlyLoggedInUser();
        Task Logout();
        Task<bool> RecheckLoginAndClearIfInvalid();
        Task<LoginResponseExtended> GetToken();
        Task<Settings> GetSettings();
        Task SetSettings(Settings settings);
        Task SetLastSeenEpisodeNumber(int episodeNumber);
        Task<int> GetLastSeenEpisodeNumber();
        Task<bool> IsLoggedInUnsafe(); // This is a workaroud, because JS calls cant be made at any time
        event EventHandler UserLoggedIn;
    }
}