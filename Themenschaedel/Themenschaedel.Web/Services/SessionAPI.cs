using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Sentry;
using Themenschaedel.Shared.Models;
using Themenschaedel.Shared.Models.Request;
using Themenschaedel.Shared.Models.Response;
using Themenschaedel.Web.Services.Interfaces;

namespace Themenschaedel.Web.Services
{
    public class SessionAPI : ISessionAPI
    {
        private readonly HttpClient _httpClient;
        private readonly Blazored.SessionStorage.ISessionStorageService _sessionStorage;

        public SessionAPI(HttpClient httpClient, Blazored.SessionStorage.ISessionStorageService sessionStorage)
        {
            _httpClient = httpClient;
            this._sessionStorage = sessionStorage;
        }

        public void DeleteSession(string token)
        {
            throw new System.NotImplementedException();
        }

        private async Task<LoginResponse> GetToken() =>
            await _sessionStorage.GetItemAsync<LoginResponse>("Token");

        public async Task<UserResponse> GetCurrentUserData()
        {
            try
            {
                using (var requestMessage =
                    new HttpRequestMessage(HttpMethod.Get, $"{_httpClient.BaseAddress}me"))
                {
                    requestMessage.Headers.Authorization =
                        new AuthenticationHeaderValue("Bearer", (await GetToken()).AccessToken);

                    var response = await _httpClient.SendAsync(requestMessage);

                    if (response.IsSuccessStatusCode)
                    {
                        string json = response.Content.ReadAsStringAsync().Result;
                        UserResponse user = JsonConvert.DeserializeObject<UserResponse>(json);
                        return user;
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            catch (Exception e)
            {
                SentrySdk.CaptureException(e);
            }

            return null;
        }

        public async Task Logout()
        {
            try
            {
                using (var requestMessage =
                    new HttpRequestMessage(HttpMethod.Get, $"{_httpClient.BaseAddress}auth/logout"))
                {
                    requestMessage.Headers.Authorization =
                        new AuthenticationHeaderValue("Bearer", (await GetToken()).AccessToken);

                    await _httpClient.SendAsync(requestMessage);
                }
            }
            catch (Exception e)
            {
                SentrySdk.CaptureException(e);
            }
        }

        public async Task<LoginResponse> RefreshToken()
        {
            try
            {
                using (var requestMessage =
                       new HttpRequestMessage(HttpMethod.Get, $"{_httpClient.BaseAddress}auth/refresh_token"))
                {
                    LoginResponse initialToken = await GetToken();
                    RefreshTokenRequest request = new RefreshTokenRequest()
                    {
                        RefreshToken = initialToken.RefreshToken,
                        UserId = initialToken.UserId
                    };

                    var response = await _httpClient.SendAsync(requestMessage);

                    if (response.IsSuccessStatusCode)
                    {
                        string json = response.Content.ReadAsStringAsync().Result;
                        Token token = JsonConvert.DeserializeObject<Token>(json);
                        LoginResponse loginResponse = new LoginResponse();
                        loginResponse = initialToken;
                        loginResponse.ValidUntil = token.ValidUntil;
                        loginResponse.AccessToken = token.Value;
                        return loginResponse;
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            catch (Exception e)
            {
                SentrySdk.CaptureException(e);
            }

            return null;
        }
    }
}