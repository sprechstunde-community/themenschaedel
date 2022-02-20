using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Themenschaedel.Shared.Models.Response;

namespace Themenschaedel.Web.Services.Interfaces
{
    public interface ISessionAPI
    {
        void DeleteSession(string token);
        Task<LoginResponseExtended> RefreshToken(LoginResponse token);
        Task<UserResponse> GetCurrentUserData(LoginResponse token);
        Task Logout(LoginResponse token);
        event EventHandler<LoginResponseExtended> NewTokenCreated;
    }
}