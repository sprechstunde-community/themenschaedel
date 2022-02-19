using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;
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
                    new HttpRequestMessage(HttpMethod.Get, $"{_httpClient.BaseAddress}auth/me"))
                {
                    LoginResponse loginResponse = await GetToken();

                    if (loginResponse == null) return null;

                    requestMessage.Headers.Authorization =
                        new AuthenticationHeaderValue("Bearer", loginResponse.AccessToken);

                    var response = await _httpClient.SendAsync(requestMessage);

                    if (response.IsSuccessStatusCode)
                    {
                        string json = response.Content.ReadAsStringAsync().Result;
                        UserResponse user = JsonSerializer.Deserialize<UserResponse>(json);
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
                    LoginResponse loginResponse =  await GetToken();

                    if (loginResponse == null) return;

                    requestMessage.Headers.Authorization =
                        new AuthenticationHeaderValue("Bearer", loginResponse.AccessToken);

                    await _httpClient.SendAsync(requestMessage);
                }
            }
            catch (Exception e)
            {
                SentrySdk.CaptureException(e);
            }
        }

        public async Task<LoginResponseExtended> RefreshToken()
        {
            try
            {
                using (var requestMessage =
                       new HttpRequestMessage(HttpMethod.Get, $"{_httpClient.BaseAddress}auth/refresh_token"))
                {
                    LoginResponse initialToken = await GetToken();
                    if (initialToken == null) return null;
                    RefreshTokenRequest request = new RefreshTokenRequest()
                    {
                        RefreshToken = initialToken.RefreshToken,
                        UserId = initialToken.UserId
                    };

                    var response = await _httpClient.SendAsync(requestMessage);

                    if (response.IsSuccessStatusCode)
                    {
                        string json = response.Content.ReadAsStringAsync().Result;
                        Token token = JsonSerializer.Deserialize<Token>(json);
                        LoginResponse loginResponse = new LoginResponse();
                        loginResponse = initialToken;
                        loginResponse.ValidUntil = token.ValidUntil;
                        loginResponse.AccessToken = token.Value;
                        return new LoginResponseExtended(loginResponse);
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