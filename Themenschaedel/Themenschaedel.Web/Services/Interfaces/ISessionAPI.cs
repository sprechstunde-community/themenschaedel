using System.Threading.Tasks;
using Themenschaedel.Shared.Props;

namespace Themenschaedel.Web.Services.Interfaces
{
    public interface ISessionAPI
    {
        void DeleteSession(string token);
        Task<ShortUser> GetCurrentUserData();
        Task Logout();
    }
}