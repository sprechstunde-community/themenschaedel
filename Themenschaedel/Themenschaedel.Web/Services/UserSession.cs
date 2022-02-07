using System;
using System.Threading.Tasks;
using Themenschaedel.Shared.Models;
using Themenschaedel.Shared.Models.Response;
using Themenschaedel.Web.Services.Interfaces;

namespace Themenschaedel.Web.Services
{
    public class UserSession : Themenschaedel.Web.Services.Interfaces.IUserSession
    {
        private readonly ISessionAPI _sessionApi;
        private readonly Blazored.SessionStorage.ISessionStorageService _sessionStorage;
        private readonly Blazored.LocalStorage.ILocalStorageService _localStorage;
        public UserResponse CurrentlyLoggedInUser { get; set; }

        private bool loggedIn = false;

        public UserSession(ISessionAPI sessionApi, Blazored.SessionStorage.ISessionStorageService sessionStorage,
            Blazored.LocalStorage.ILocalStorageService localStorage)
        {
            this._sessionApi = sessionApi;
            this._sessionStorage = sessionStorage;
            this._localStorage = localStorage;
        }

        #region non-async

        public bool IsLoggedIn()
        {
            throw new System.NotImplementedException();
        }

        #endregion non-async


        #region async

        public async Task<LoginResponse> GetToken()
        {
            LoginResponse token = await _sessionStorage.GetItemAsync<LoginResponse>("Token");
            if (token == null) token = await GetLocalStorageTokenAndSetSessionToken();
            if (token == null) return null;
            if ((token.ValidUntil - DateTime.Now).TotalMinutes <= 5)
            {
                await SetToken(await _sessionApi.RefreshToken());
                token = await _sessionStorage.GetItemAsync<LoginResponse>("Token");
            }
            return token;
        }

        public async Task<Settings> GetSettings() => await _sessionStorage.GetItemAsync<Settings>("Settings");

        public async Task SetSettings(Settings settings)
        {
            await _sessionStorage.SetItemAsync("Settings", settings);
        }

        private async Task<LoginResponse> GetLocalStorageTokenAndSetSessionToken()
        {
            LoginResponse token = await _localStorage.GetItemAsync<LoginResponse>("Token");
            if (token != null && CurrentlyLoggedInUser == null)
            {
                if ((token.ValidUntil - DateTime.Now).TotalMinutes <= 5)
                {
                    await SetToken(await _sessionApi.RefreshToken());
                    token = await _sessionStorage.GetItemAsync<LoginResponse>("Token");
                }
                else
                {
                    await _sessionStorage.SetItemAsync("Token", token);
                }
            }
            return token;
        }

        private async Task SetToken(LoginResponse token, bool keepLoggedIn = false)
        {
            if (keepLoggedIn)
            {
                await _localStorage.SetItemAsync("Token", token);
            }

            await _sessionStorage.SetItemAsync("Token", token);
            bool isLoggedIn = await IsLoggedInAsync();
            if (isLoggedIn)
            {
                await PopulateUserObject();
            }
        }

        private async Task ClearToken()
        {
            await _sessionStorage.RemoveItemAsync("Token");
            await _localStorage.RemoveItemAsync("Token");
        }

        public async Task SetAuthenticationTokenAsync(LoginResponse authenticationToken, bool keepLoggedIn)
        {
            await SetToken(authenticationToken, keepLoggedIn);
            loggedIn = true;
        }

        private async Task PopulateUserObject()
        {
            this.CurrentlyLoggedInUser = await _sessionApi.GetCurrentUserData();
            if (this.CurrentlyLoggedInUser == null)
            {
                await RecheckLoginAndClearIfInvalid();
            }
        }

        public async Task<bool> IsLoggedInAsync()
        {
            LoginResponse token = await GetToken();
            if (token != null)
            {
                if (token.AccessToken != null && token.TokenType != null)
                {
                    if (CurrentlyLoggedInUser == null)
                    {
                        await PopulateUserObject();
                    }

                    return true;
                }
            }

            return false;
        }

        public async Task<bool> RecheckLoginAndClearIfInvalid()
        {
            bool valid = await CheckToken();
            if (!valid) await ClearUserData();
            return valid;
        }

        private async Task<bool> CheckToken()
        {
            UserResponse currentUser = await _sessionApi.GetCurrentUserData();
            if (currentUser == null) return false;
            if (String.IsNullOrEmpty(currentUser.Username)) return false;
            if (currentUser != CurrentlyLoggedInUser) return false;

            return true;
        }


        public async Task<UserResponse> GetCurrentlyLoggedInUser() => CurrentlyLoggedInUser;

        private async Task ClearUserData()
        {
            CurrentlyLoggedInUser = null;
            bool loggedIn = false;
            await ClearToken();
        }

        public async Task Logout()
        {
            if (await RecheckLoginAndClearIfInvalid())
            {
                await _sessionApi.Logout();
            }
        }

        #endregion async
    }
}