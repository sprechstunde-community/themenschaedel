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

        private bool saving = false;

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

        protected async Task SaveTopics()
        {
            await SendTopic();
            this.StateHasChanged();
        }

        protected async Task AddExtraTimeToClaim()
        {
            await _data.AddExtraTimeToClaim();
            this.StateHasChanged();
        }

        protected async Task FinalizeClaim()
        {
            await _data.FinalizeClaim();
            this.StateHasChanged();
        }


        protected async Task RemoveTopic(TopicPostRequestClient topic)
        {
            localTopics.Remove(topic);
            this.StateHasChanged();
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
            if (saving) return;

            saving = true;
            List<TopicPostRequest> topics = new List<TopicPostRequest>();
            foreach (TopicPostRequestClient item in localTopics)
            {
                topics.Add(new TopicPostRequest(item));
            }

            if (topics.Count == 0) await _data.PostTopic(new List<TopicPostRequest>(), new List<PeopleInEpisode>());
            else await _data.PostTopic(topics, new List<PeopleInEpisode>());
            _toastService.ShowSuccess($"Topcis saved successfully.");
            saving = false;
        }
    }
}