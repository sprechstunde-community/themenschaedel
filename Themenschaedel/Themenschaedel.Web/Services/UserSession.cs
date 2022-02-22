using System;
using System.Threading.Tasks;
using Themenschaedel.Shared.Models;
using Themenschaedel.Shared.Models.Response;
using Themenschaedel.Web.Services.Interfaces;

namespace Themenschaedel.Web.Services
{
    public enum LoginDuration
    {
        KeepLoggedIn,
        Temporary,
        None
    }

    public class UserSession : Themenschaedel.Web.Services.Interfaces.IUserSession
    {
        private readonly ISessionAPI _sessionApi;
        private readonly Blazored.SessionStorage.ISessionStorageService _sessionStorage;
        private readonly Blazored.LocalStorage.ILocalStorageService _localStorage;

        public event EventHandler UserLoggedIn;
        public UserResponse CurrentlyLoggedInUser { get; set; }

        private bool loggedIn = false;

        public UserSession(ISessionAPI sessionApi, Blazored.SessionStorage.ISessionStorageService sessionStorage,
            Blazored.LocalStorage.ILocalStorageService localStorage)
        {
            this._sessionApi = sessionApi;
            this._sessionStorage = sessionStorage;
            this._localStorage = localStorage;

            _sessionApi.NewTokenCreated += async (s, e) =>
            {
                await SessionApiOnNewTokenCreated(s, e);
            };
        }

        #region non-async

        public bool IsLoggedIn()
        {
            throw new System.NotImplementedException();
        }
        #endregion non-async


        #region async

        public async Task SetLastSeenEpisodeNumber(int episodeNumber)
        {
            await _localStorage.SetItemAsync("LastSeenEpisode", episodeNumber);
        }

        public async Task<int> GetLastSeenEpisodeNumber()
        {
            try
            {
                return await _localStorage.GetItemAsync<int>("LastSeenEpisode");
            }
            catch (Exception e)
            {
                return 0;
            }
        }

        public async Task<LoginResponseExtended> GetToken()
        {
            LoginResponseExtended token = await _sessionStorage.GetItemAsync<LoginResponseExtended>("Token");
            if (token == null) token = await GetLocalStorageTokenAndSetSessionToken();
            if (token == null) return null;
            if ((token.ValidUntil - DateTime.Now).TotalMinutes <= 5)
            {
                await SetToken(await _sessionApi.RefreshToken(token), LoginDuration.None);
                token = await _sessionStorage.GetItemAsync<LoginResponseExtended>("Token");
            }
            return token;
        }

        public async Task<Settings> GetSettings() => await _sessionStorage.GetItemAsync<Settings>("Settings");

        public async Task SetSettings(Settings settings)
        {
            await _sessionStorage.SetItemAsync("Settings", settings);
        }

        private async Task<LoginResponseExtended> GetLocalStorageTokenAndSetSessionToken()
        {
            LoginResponseExtended token = await _localStorage.GetItemAsync<LoginResponseExtended>("Token");
            if (token != null && CurrentlyLoggedInUser == null)
            {
                // This checks if the session is about to expire, if so logout the user and clear all tokens.
                if ((token.SessionExpirationDate - DateTime.Now).TotalMinutes <= 1)
                {
                    await _sessionApi.Logout(token);
                    await ClearToken();
                    return null;
                }

                if ((token.ValidUntil - DateTime.Now).TotalMinutes <= 5)
                {
                    // This checks if the current localStorage Token will expire in less than 5 minutes, if so it will extend it, by refreshing the token
                    await SetToken(await _sessionApi.RefreshToken(token), LoginDuration.None);
                    token = await _sessionStorage.GetItemAsync<LoginResponseExtended>("Token");
                }
                else
                {
                    await _sessionStorage.SetItemAsync("Token", token);
                }
            }
            return token;
        }

        private async Task SetToken(LoginResponse tokenResponse, LoginDuration keepLoggedIn = LoginDuration.Temporary)
        {
            LoginResponseExtended token = new LoginResponseExtended(tokenResponse);


            switch (keepLoggedIn)
            {
                case LoginDuration.Temporary:
                    token.SessionExpirationDate = DateTime.Now.AddHours(3);
                    await _localStorage.SetItemAsync("Token", token);
                    break;
                case LoginDuration.KeepLoggedIn:
                    token.SessionExpirationDate = DateTime.MaxValue;
                    await _localStorage.SetItemAsync("Token", token);
                    break;
                case LoginDuration.None:
                    break;
            }

            await _sessionStorage.SetItemAsync("Token", token);
            bool isLoggedIn = await IsLoggedInAsync();
            if (isLoggedIn)
            {
                UserLoggedIn?.Invoke(null, null);
                await PopulateUserObject();
            }
        }

        private async Task ClearToken()
        {
            await _sessionStorage.RemoveItemAsync("Token");
            await _localStorage.RemoveItemAsync("Token");
            loggedIn = false;
        }

        public async Task SetAuthenticationTokenAsync(LoginResponse authenticationToken, LoginDuration keepLoggedIn)
        {
            await SetToken(authenticationToken, keepLoggedIn);
            loggedIn = true;
        }

        private async Task PopulateUserObject()
        {
            this.CurrentlyLoggedInUser = await _sessionApi.GetCurrentUserData(await GetToken());
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
                        loggedIn = true;
                        UserLoggedIn?.Invoke(null, null);
                        await PopulateUserObject();
                    }

                    return true;
                }
            }

            return false;
        }

        public async Task<bool> IsLoggedInUnsafe()
        {
            return loggedIn;
        }

        public async Task<bool> RecheckLoginAndClearIfInvalid()
        {
            bool valid = await CheckToken();
            if (!valid) await ClearUserData();
            return valid;
        }

        private async Task<bool> CheckToken()
        {
            UserResponse currentUser = await _sessionApi.GetCurrentUserData(await GetToken());
            if (currentUser == null) return false;
            if (String.IsNullOrEmpty(currentUser.Username)) return false;
            if (currentUser != CurrentlyLoggedInUser) return false;
            loggedIn = true;
            UserLoggedIn?.Invoke(null, null);

            return true;
        }


        public async Task<UserResponse> GetCurrentlyLoggedInUser()
        {
            if (CurrentlyLoggedInUser == null)
            {
                if (await IsLoggedInAsync())
                {
                    await PopulateUserObject();
                    return CurrentlyLoggedInUser;
                }
            }
            else
            {
                return CurrentlyLoggedInUser;
            }
            return CurrentlyLoggedInUser;
        }

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
                loggedIn = false;
                await _sessionApi.Logout(await GetToken());
            }
        }

        private async Task SessionApiOnNewTokenCreated(object? sender, LoginResponseExtended e)
        {
            await SetToken(e, LoginDuration.None);
        }
        #endregion async
    }
}