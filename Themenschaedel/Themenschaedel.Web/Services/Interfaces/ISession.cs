using System.Threading.Tasks;
using Themenschaedel.Shared.Props;

namespace Themenschaedel.Web.Services.Interfaces
{
    public interface ISession
    {
        Task SetAuthenticationTokenAsync(Token authenticationToken, bool keepLoggedIn);
        bool IsLoggedIn();
        Task<bool> IsLoggedInAsync();
        Task<ShortUser> GetCurrentlyLoggedInUser();
        Task Logout();
        Task<bool> RecheckLoginAndClearIfInvalid();
        Task<Token> GetToken();
    }
}