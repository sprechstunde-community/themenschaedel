using System;
using System.Threading.Tasks;
using Themenschaedel.Shared.Props;
using Themenschaedel.Web.Services.Interfaces;

namespace Themenschaedel.Web.Services
{
    public class UserSession : Themenschaedel.Web.Services.Interfaces.IUserSession
    {
        private readonly ISessionAPI _sessionApi;
        private readonly Blazored.SessionStorage.ISessionStorageService _sessionStorage;
        private readonly Blazored.LocalStorage.ILocalStorageService _localStorage;
        public ShortUser CurrentlyLoggedInUser { get; set; }

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

        public async Task<Token> GetToken()
        {
            Token token = await _sessionStorage.GetItemAsync<Token>("Token");
            if (token == null) await GetLocalStorageTokenAndSetSessionToken();
            return await _sessionStorage.GetItemAsync<Token>("Token");
        }

        public async Task<Settings> GetSettings() => await _sessionStorage.GetItemAsync<Settings>("Settings");

        public async Task SetSettings(Settings settings)
        {
            await _sessionStorage.SetItemAsync("Settings", settings);
        }

        private async Task<Token> GetLocalStorageTokenAndSetSessionToken()
        {
            Token token = await _localStorage.GetItemAsync<Token>("Token");
            if (token != null && CurrentlyLoggedInUser == null)
            {
                await _sessionStorage.SetItemAsync("Token", token);
            }
            return token;
        }

        private async Task SetToken(Token token, bool keepLoggedIn)
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
            await _sessionStorage.SetItemAsync("Token", new Token { access_token = null, token_type = null });
            await _localStorage.SetItemAsync("Token", new Token { access_token = null, token_type = null });
        }

        public async Task SetAuthenticationTokenAsync(Token authenticationToken, bool keepLoggedIn)
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
            Token token = await GetToken();
            if (token != null)
            {
                if (token.access_token != null && token.token_type != null)
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
            ShortUser currentUser = await _sessionApi.GetCurrentUserData();
            if (currentUser == null) return false;
            if (String.IsNullOrEmpty(currentUser.username)) return false;
            if (currentUser != CurrentlyLoggedInUser) return false;

            return true;
        }


        public async Task<ShortUser> GetCurrentlyLoggedInUser() => CurrentlyLoggedInUser;

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
                _sessionApi.Logout();
            }
        }

        #endregion async
    }
}