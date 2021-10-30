using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Sentry;
using Themenschaedel.Shared.Props;
using Themenschaedel.Web.Services.Interfaces;
using User = Themenschaedel.Shared.Props.User;

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

        private async Task<Token> GetToken() =>
            await _sessionStorage.GetItemAsync<Token>("Token");

        public async Task<ShortUser> GetCurrentUserData()
        {
            try
            {
                using (var requestMessage =
                    new HttpRequestMessage(HttpMethod.Get, $"{_httpClient.BaseAddress}me"))
                {
                    requestMessage.Headers.Authorization =
                        new AuthenticationHeaderValue("Bearer", (await GetToken()).access_token);

                    var response = await _httpClient.SendAsync(requestMessage);

                    if (response.IsSuccessStatusCode)
                    {
                        string json = response.Content.ReadAsStringAsync().Result;
                        ShortUserWrapper user = JsonConvert.DeserializeObject<ShortUserWrapper>(json);
                        return user.data;
                    }else if (response.RequestMessage.RequestUri == new Uri("https://account.schaedel.rocks/login"))
                    {
                        return null;
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
                        new AuthenticationHeaderValue("Bearer", (await GetToken()).access_token);

                    await _httpClient.SendAsync(requestMessage);
                }
            }
            catch (Exception e)
            {
                SentrySdk.CaptureException(e);
            }
        }
    }
}