using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Blazored.Toast.Services;
using Sentry;
using Themenschaedel.Shared.Props;
using Themenschaedel.Web.Services.Interfaces;

namespace Themenschaedel.Web.Services
{
    public class ApiData : IData
    {
        private const string APPLICATION_TITLE = "Themenschaedel-Web";

        private readonly HttpClient _httpClient;
        private readonly Themenschaedel.Web.Services.Interfaces.ISession _session;
        private readonly IToastService _toastService;

        public ApiData(HttpClient httpClient, Themenschaedel.Web.Services.Interfaces.ISession session,
            IToastService toastService)
        {
            _httpClient = httpClient;
            _session = session;
            _toastService = toastService;
        }


        public async Task<Episode> GetEpisode(int id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_httpClient.BaseAddress}episodes/{id}");
                if (response.IsSuccessStatusCode)
                {
                    string json = response.Content.ReadAsStringAsync().Result;
                    GetEpisodeWorkaround ep = JsonConvert.DeserializeObject<GetEpisodeWorkaround>(json);
                    return ep.data;
                }
            }
            catch (Exception e)
            {
                SentrySdk.CaptureException(e);
            }

            return null;
        }

        public async Task<Token> Login(string username, string password, bool keepLoggedIn)
        {
            try
            {
                await _session.Logout();
                string json = "{ \"username\": \"" + username + "\", \"password\": \"" + password +
                              "\", \"application_name\": \"" + APPLICATION_TITLE + "\"}";
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                Uri uriExecution = new Uri($"{_httpClient.BaseAddress}auth/login");
                var response = await _httpClient.PostAsync(uriExecution, content);
                Token responseObject = await response.Content.ReadFromJsonAsync<Token>();
                if (responseObject.access_token == null && responseObject.token_type == null)
                {
                    _toastService.ShowError("Login failed: email or password are wrong!");
                    return null;
                }
                else
                {
                    await _session.SetAuthenticationTokenAsync(responseObject, keepLoggedIn);
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

        public async Task<GetTopicWorkaroundWrapper> GetTopics(int EpisodeID)
        {
            try
            {
                var response =
                    await _httpClient.GetAsync(
                        $"{_httpClient.BaseAddress}episodes/{EpisodeID.ToString()}/topics?page=0&per_page=999");
                if (response.IsSuccessStatusCode)
                {
                    string json = response.Content.ReadAsStringAsync().Result;
                    GetTopicWorkaroundWrapper tp = JsonConvert.DeserializeObject<GetTopicWorkaroundWrapper>(json);
                    return tp;
                }
            }
            catch (Exception e)
            {
                SentrySdk.CaptureException(e);
            }

            return null;
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
                        new AuthenticationHeaderValue("Bearer", (await _session.GetToken()).access_token);

                    await _httpClient.SendAsync(requestMessage);
                }
            }
            catch (Exception e)
            {
                SentrySdk.CaptureException(e);
            }
        }

        public async Task<Topic> AddTopic(Topic topic, int episodeID)
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
                        new AuthenticationHeaderValue("Bearer", (await _session.GetToken()).access_token);

                    string topicJson = JsonConvert.SerializeObject(new
                    {
                        name = topic.name, start = topic.start, end = topic.end, ad = topic.ad,
                        community_contribution = topic.community_contribution
                    });
                    StringContent stringContent = new StringContent(topicJson, Encoding.UTF8, "application/json");
                    requestMessage.Content = stringContent;

                    response = await _httpClient.SendAsync(requestMessage);
                }

                if (response != null)
                {
                    if (response.IsSuccessStatusCode)
                    {
                        // Get created topic information
                        string json = response.Content.ReadAsStringAsync().Result;
                        TopicResponse topicResponse =
                            JsonConvert.DeserializeObject<TopicResponse>(json);

                        // Send Subtopics to API
                        foreach (Subtopics subtopic in topic.subtopics)
                        {
                            await AddSubtopic(subtopic, topicResponse.data.id);
                        }

                        // Get the servers current state of the object and return it
                        return await GetTopic(topicResponse.data.id);
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

        private async Task<Subtopics> AddSubtopic(Subtopics subtopic, int topicID)
        {
            try
            {
                HttpResponseMessage response = null;
                
                using (var requestMessage =
                    new HttpRequestMessage(HttpMethod.Post,
                        $"{_httpClient.BaseAddress}topics/{topicID.ToString()}/subtopics"))
                {
                    requestMessage.Headers.Authorization =
                        new AuthenticationHeaderValue("Bearer", (await _session.GetToken()).access_token);

                    string subtopicJson = JsonConvert.SerializeObject(new { name = subtopic.name });
                    StringContent stringContent = new StringContent(subtopicJson, Encoding.UTF8, "application/json");
                    requestMessage.Content = stringContent;

                    response = await _httpClient.SendAsync(requestMessage);
                }

                if (response != null)
                {
                    if (response.IsSuccessStatusCode)
                    {
                        string json = response.Content.ReadAsStringAsync().Result;
                        SubtopicResponse topicResponseWorkaround =
                            JsonConvert.DeserializeObject<SubtopicResponse>(json);
                        return topicResponseWorkaround.data;
                    }
                }
            }
            catch (Exception e)
            {
                SentrySdk.CaptureException(e);
            }

            return null;
        }
        
        private async Task<Subtopics> UpdateSubtopic(Subtopics subtopic)
        {
            try
            {
                HttpResponseMessage response = null;
                
                using (var requestMessage =
                    new HttpRequestMessage(HttpMethod.Put,
                        $"{_httpClient.BaseAddress}subtopics/{subtopic.id.ToString()}"))
                {
                    requestMessage.Headers.Authorization =
                        new AuthenticationHeaderValue("Bearer", (await _session.GetToken()).access_token);

                    string subtopicJson = JsonConvert.SerializeObject(new { name = subtopic.name });
                    StringContent stringContent = new StringContent(subtopicJson, Encoding.UTF8, "application/json");
                    requestMessage.Content = stringContent;

                    response = await _httpClient.SendAsync(requestMessage);
                }

                if (response != null)
                {
                    if (response.IsSuccessStatusCode)
                    {
                        string json = response.Content.ReadAsStringAsync().Result;
                        SubtopicResponse topicResponseWorkaround =
                            JsonConvert.DeserializeObject<SubtopicResponse>(json);
                        return topicResponseWorkaround.data;
                    }
                }
            }
            catch (Exception e)
            {
                SentrySdk.CaptureException(e);
            }

            return null;
        }

        public async Task<Topic> UpdateTopic(Topic topic)
        {
            try
            {
                HttpResponseMessage response = null;

                // Send Topic to API
                using (var requestMessage = new HttpRequestMessage(HttpMethod.Put,
                    $"{_httpClient.BaseAddress}topics/{topic.id.ToString()}"))
                {
                    requestMessage.Headers.Authorization =
                        new AuthenticationHeaderValue("Bearer", (await _session.GetToken()).access_token);

                    string json = JsonConvert.SerializeObject(new
                    {
                        name = topic.name, start = topic.start, end = topic.end, ad = topic.ad,
                        community_contribution = topic.community_contribution
                    });
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
                        TopicResponse topicResponse =
                            JsonConvert.DeserializeObject<TopicResponse>(json);
                        
                        // Send Subtopics to API
                        foreach (Subtopics subtopic in topic.subtopics)
                        {
                            if (subtopic.id == 0)
                            {
                                await AddSubtopic(subtopic, topicResponse.data.id);
                            }
                            else
                            {
                                await UpdateSubtopic(subtopic);
                            }
                        }

                        // Get the servers current state of the object and return it
                        return await GetTopic(topicResponse.data.id);
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

        private async Task<Topic> GetTopic(int topicID)
        {
            try
            {
                HttpResponseMessage response = null;

                using (var requestMessage =
                    new HttpRequestMessage(HttpMethod.Get, $"{_httpClient.BaseAddress}topics/{topicID.ToString()}"))
                {
                    requestMessage.Headers.Authorization =
                        new AuthenticationHeaderValue("Bearer", (await _session.GetToken()).access_token);

                    response = await _httpClient.SendAsync(requestMessage);
                }

                if (response != null)
                {
                    if (response.IsSuccessStatusCode)
                    {
                        // Get created topic information
                        string json = response.Content.ReadAsStringAsync().Result;
                        GetTopicWorkaround topicResponseWorkaround =
                            JsonConvert.DeserializeObject<GetTopicWorkaround>(json);
                        return topicResponseWorkaround.data;
                    }
                }
            }
            catch (Exception e)
            {
                SentrySdk.CaptureException(e);
            }

            return null;
        }

        public async Task<GetEpisodesWorkaround> GetEpisodes(int count, int page)
        {
            try
            {
                var response =
                    await _httpClient.GetAsync(
                        $"{_httpClient.BaseAddress}episodes?page={page.ToString()}&per_page={count.ToString()}");
                if (response.IsSuccessStatusCode)
                {
                    string json = response.Content.ReadAsStringAsync().Result;
                    GetEpisodesWorkaround ep = JsonConvert.DeserializeObject<GetEpisodesWorkaround>(json);
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
        
        public async Task DeleteTopic(Topic topic)
        {
            try
            {
                foreach (Subtopics subtopic in topic.subtopics)
                {
                    await DeleteSubtopic(subtopic);
                }
                using (var requestMessage =
                    new HttpRequestMessage(HttpMethod.Delete, $"{_httpClient.BaseAddress}topics/{topic.id.ToString()}"))
                {
                    requestMessage.Headers.Authorization =
                        new AuthenticationHeaderValue("Bearer", (await _session.GetToken()).access_token);

                    HttpResponseMessage response = await _httpClient.SendAsync(requestMessage);
                }
            }
            catch (Exception e)
            {
                SentrySdk.CaptureException(e);
            }
        }

        public async Task DeleteSubtopic(Subtopics subtopic)
        {
            try
            {
                using (var requestMessage =
                    new HttpRequestMessage(HttpMethod.Delete, $"{_httpClient.BaseAddress}subtopics/{subtopic.id.ToString()}"))
                {
                    requestMessage.Headers.Authorization =
                        new AuthenticationHeaderValue("Bearer", (await _session.GetToken()).access_token);

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