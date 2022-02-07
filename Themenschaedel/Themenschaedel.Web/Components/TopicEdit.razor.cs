using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Blazored.Toast.Services;
using Microsoft.AspNetCore.Components;
using Themenschaedel.Shared.Models;
using Themenschaedel.Shared.Models.Request;
using Themenschaedel.Web.Services.Interfaces;

namespace Themenschaedel.Components
{
    public partial class TopicEdit : ComponentBase
    {
        [Parameter] [Required] public int episodeId { get; set; }
        [Parameter] public EpisodeClientExtra episode { get; set; }

        [Inject] protected IData _data { get; set; }
        [Inject] protected IToastService _toastService { get; set; }

        protected List<TopicPostRequestClient> localTopics = new List<TopicPostRequestClient>();
        protected override async Task OnInitializedAsync()
        {
            await PopulateTopics();
        }

        protected async Task AddTopic()
        {
            localTopics.Add(new TopicPostRequestClient());
            this.StateHasChanged();
        }


        protected async Task RemoveTopic(TopicPostRequestClient topic)
        {
            localTopics.Remove(topic);
            this.StateHasChanged();
            if (String.IsNullOrEmpty(topic.Name)) _toastService.ShowSuccess($"Delted Topic: [Unnamed Topic].");
            else _toastService.ShowSuccess($"Delted Topic: {topic.Name}.");
        }

        protected async Task PopulateTopics()
        {
            for (int i = 0; i < episode.Topic.Count; i++)
            {
                localTopics.Add(new TopicPostRequestClient(episode.Topic[i]));
            }
        }

        protected async Task SendTopic()
        {
            List<TopicPostRequest> topics = new List<TopicPostRequest>();
            foreach (TopicPostRequestClient item in localTopics)
            {
                topics.Add(new TopicPostRequest(item));
            }
            await _data.PostTopic(topics, new List<PeopleInEpisode>());
        }
    }
}