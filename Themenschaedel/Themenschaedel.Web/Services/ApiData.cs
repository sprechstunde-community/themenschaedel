using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Blazored.Toast.Services;
using Sentry;
using Themenschaedel.Shared.Models;
using Themenschaedel.Shared.Models.Request;
using Themenschaedel.Shared.Models.Response;
using Themenschaedel.Web.Services.Interfaces;
using User = Themenschaedel.Shared.Models.User;

namespace Themenschaedel.Web.Services
{
    public class ApiData : IData
    {
        private readonly HttpClient _httpClient;
        private readonly Themenschaedel.Web.Services.Interfaces.IUserSession _userSession;
        private readonly IToastService _toastService;

        public ApiData(HttpClient httpClient, Themenschaedel.Web.Services.Interfaces.IUserSession userSession,
            IToastService toastService)
        {
            _httpClient = httpClient;
            _userSession = userSession;
            _toastService = toastService;
        }

        public async Task ClaimEpisode(int episodeID)
        {
            try
            {
                using (var requestMessage =
                       new HttpRequestMessage(HttpMethod.Post,
                           $"{_httpClient.BaseAddress}episodes/${episodeID.ToString()}/claim"))
                {
                    requestMessage.Headers.Authorization =
                        new AuthenticationHeaderValue("Bearer", (await _userSession.GetToken()).AccessToken);

                    await _httpClient.SendAsync(requestMessage);
                }
            }
            catch (Exception e)
            {
                SentrySdk.CaptureException(e);
            }
        }

        public async Task<Themenschaedel.Shared.Models.Episode> GetEpisode(int id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_httpClient.BaseAddress}episodes/{id}");
                if (response.IsSuccessStatusCode)
                {
                    string json = response.Content.ReadAsStringAsync().Result;
                    EpisodeExtendedExtra ep = JsonSerializer.Deserialize<EpisodeExtendedExtra>(json);
                    return ep;
                }
            }
            catch (Exception e)
            {
                SentrySdk.CaptureException(e);
            }

            return null;
        }

        public async Task<EpisodeResponse> GetEpisodes(int count, int page)
        {
            try
            {
                var response =
                    await _httpClient.GetAsync(
                        $"{_httpClient.BaseAddress}episodes?page={page.ToString()}&per_page={count.ToString()}");
                if (response.IsSuccessStatusCode)
                {
                    string json = response.Content.ReadAsStringAsync().Result;
                    EpisodeResponse ep = JsonSerializer.Deserialize<EpisodeResponse>(json);
                    return ep;
                }

                if (response.StatusCode == HttpStatusCode.NotFound)
                {
                    _toastService.ShowError("Error 404: Backend server is offline.");
                    SentrySdk.CaptureMessage("API: Error 404");
                }
            }
            catch (Exception e)
            {
                SentrySdk.CaptureException(e);
            }

            return null;
        }

        public async Task<List<TopicExtended>> GetTopics(int EpisodeID)
        {
            try
            {
                var response =
                    await _httpClient.GetAsync(
                        $"{_httpClient.BaseAddress}episodes/{EpisodeID.ToString()}/topics?page=0&per_page=999");
                if (response.IsSuccessStatusCode)
                {
                    string json = response.Content.ReadAsStringAsync().Result;
                    List<TopicExtended> tp = JsonSerializer.Deserialize<List<TopicExtended>>(json);
                    return tp;
                }
            }
            catch (Exception e)
            {
                SentrySdk.CaptureException(e);
            }

            return null;
        }

        public async Task<LoginResponse> Login(string username, string password, bool keepLoggedIn)
        {
            try
            {
                await _userSession.Logout();
                UserLogin loginData = new UserLogin()
                {
                    Username = username,
                    Password = password
                };
                string json = JsonSerializer.Serialize(loginData);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                Uri uriExecution = new Uri($"{_httpClient.BaseAddress}auth/login");
                var response = await _httpClient.PostAsync(uriExecution, content);
                LoginResponse responseObject = await response.Content.ReadFromJsonAsync<LoginResponse>();
                if (responseObject.AccessToken == null && responseObject.TokenType == null)
                {
                    _toastService.ShowError("Login failed: email or password are wrong!");
                    return null;
                }
                else
                {
                    await _userSession.SetAuthenticationTokenAsync(responseObject, keepLoggedIn);
                    return responseObject;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Login failed");
                SentrySdk.CaptureException(e);
            }

            return null;
        }

        public async Task<List<TopicExtended>> PostTopic(List<TopicPostRequest> topic, List<PeopleInEpisode> people, int episodeID)
        {
            try
            {
                HttpResponseMessage response = null;

                // Send Topic to API
                using (var requestMessage =
                    new HttpRequestMessage(HttpMethod.Post,
                        $"{_httpClient.BaseAddress}episodes/{episodeID.ToString()}/topics"))
                {
                    requestMessage.Headers.Authorization =
                        new AuthenticationHeaderValue("Bearer", (await _userSession.GetToken()).AccessToken);

                    TopicRequest request = new TopicRequest(people, topic);

                    string json = JsonSerializer.Serialize(request);

                    StringContent stringContent = new StringContent(json, Encoding.UTF8, "application/json");
                    requestMessage.Content = stringContent;

                    response = await _httpClient.SendAsync(requestMessage);
                }

                if (response != null)
                {
                    if (response.IsSuccessStatusCode)
                    {
                        // Get created topic information
                        string json = response.Content.ReadAsStringAsync().Result;
                        List<TopicExtended> topicResponse =
                            JsonSerializer.Deserialize<List<TopicExtended>>(json);

                        // Get the servers current state of the object and return it
                        return topicResponse;
                    }
                }

                if (response.StatusCode == HttpStatusCode.NotFound)
                {
                    _toastService.ShowError("Error 404: Backend server is offline.");
                    SentrySdk.CaptureMessage("API: Error 404");
                }
            }
            catch (Exception e)
            {
                SentrySdk.CaptureException(e);
            }

            return null;
        }

        public async Task<List<Themenschaedel.Shared.Models.Search>> Search(string searchTerm)
        {
            throw new NotImplementedException();
        }
    }
}
