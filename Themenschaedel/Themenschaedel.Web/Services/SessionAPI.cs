using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
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

        public event EventHandler<LoginResponseExtended> NewTokenCreated;

        public SessionAPI(HttpClient httpClient, Blazored.SessionStorage.ISessionStorageService sessionStorage)
        {
            _httpClient = httpClient;
            this._sessionStorage = sessionStorage;
        }

        public void DeleteSession(string token)
        {
            throw new System.NotImplementedException();
        }

        public async Task<UserResponse> GetCurrentUserData(LoginResponse token)
        {
            UserResponse response = null;
            try
            {
                response = await GetCurrentUserDataRequest(token);
            }
            catch (TokenDoesNotExistException e)
            {
                LoginResponse refreshedToken = await RefreshToken(token);
                response = await GetCurrentUserDataRequest(token);
            }

            return response;
        }

        private async Task<UserResponse> GetCurrentUserDataRequest(LoginResponse token)
        {
            bool unauthorizedRequest = false;
            try
            {
                using (var requestMessage =
                    new HttpRequestMessage(HttpMethod.Get, $"{_httpClient.BaseAddress}auth/me"))
                {
                    if (token == null) return null;

                    requestMessage.Headers.Authorization =
                        new AuthenticationHeaderValue("Bearer", token.AccessToken);

                    var response = await _httpClient.SendAsync(requestMessage);

                    if (response.IsSuccessStatusCode)
                    {
                        string json = response.Content.ReadAsStringAsync().Result;
                        UserResponse user = JsonSerializer.Deserialize<UserResponse>(json);
                        return user;
                    }
                    else
                    {
                        if (response.StatusCode == HttpStatusCode.Unauthorized) unauthorizedRequest = true;
                    }
                }
            }
            catch (Exception e)
            {
                SentrySdk.CaptureException(e);
            }

            if (unauthorizedRequest) throw new TokenDoesNotExistException();
            return null;
        }

        public async Task Logout(LoginResponse token)
        {
            try
            {
                using (var requestMessage =
                    new HttpRequestMessage(HttpMethod.Get, $"{_httpClient.BaseAddress}auth/logout"))
                {
                    if (token == null) return;

                    requestMessage.Headers.Authorization =
                        new AuthenticationHeaderValue("Bearer", token.AccessToken);

                    await _httpClient.SendAsync(requestMessage);
                }
            }
            catch (Exception e)
            {
                SentrySdk.CaptureException(e);
            }
        }

        public async Task<LoginResponseExtended> RefreshToken(LoginResponse token)
        {
            try
            {
                using (var requestMessage =
                       new HttpRequestMessage(HttpMethod.Post, $"{_httpClient.BaseAddress}auth/refresh_token"))
                {
                    if (token == null) return null;
                    RefreshTokenRequest request = new RefreshTokenRequest()
                    {
                        RefreshToken = token.RefreshToken,
                        UserId = token.UserId
                    };

                    string jsonRequest = JsonSerializer.Serialize(request);

                    StringContent stringContent = new StringContent(jsonRequest, Encoding.UTF8, "application/json");
                    requestMessage.Content = stringContent;

                    var response = await _httpClient.SendAsync(requestMessage);

                    if (response.IsSuccessStatusCode)
                    {
                        string json = response.Content.ReadAsStringAsync().Result;
                        Token tokenResponse = JsonSerializer.Deserialize<Token>(json);
                        LoginResponse loginResponse = new LoginResponse();
                        loginResponse = token;
                        loginResponse.ValidUntil = tokenResponse.ValidUntil;
                        loginResponse.AccessToken = tokenResponse.Value;
                        LoginResponseExtended loginReponseExtended = new LoginResponseExtended(loginResponse);
                        NewTokenCreated?.Invoke(null, loginReponseExtended);
                        return loginReponseExtended;
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


    [Serializable]
    public class TokenDoesNotExistException : Exception
    {
        public TokenDoesNotExistException()
        { }

        public TokenDoesNotExistException(string message)
            : base(message)
        { }

        public TokenDoesNotExistException(string message, Exception innerException)
            : base(message, innerException)
        { }
    }
}