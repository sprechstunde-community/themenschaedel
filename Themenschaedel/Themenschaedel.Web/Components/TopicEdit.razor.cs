using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
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
        [Parameter] public EventCallback FinalizeTopic { get; set; }
        [Parameter] public EpisodeClientExtra episode { get; set; }

        [Inject] protected IData _data { get; set; }
        [Inject] protected IToastService _toastService { get; set; }

        private bool saving = false;

        protected List<TopicPostRequestClient> localTopics = new List<TopicPostRequestClient>();

        protected DateTime ClaimValidUntil;
        protected string ValidRemaining;

        protected static System.Timers.Timer ValidTimeRemainingTimer;
        protected override async Task OnInitializedAsync()
        {
            await PopulateTopics();
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                EpisodeWithValidUntilClaim claimedEpisode = await _data.GetClaimedEpisode();
                ClaimValidUntil = claimedEpisode.valid_until;

                ValidTimeRemainingTimer = new System.Timers.Timer(1000);
                ValidTimeRemainingTimer.Elapsed += CountDownTimer;
                ValidTimeRemainingTimer.Enabled = true;
            }
        }

        public void CountDownTimer(Object source, ElapsedEventArgs e)
        {
            double timeLeftDouble = (ClaimValidUntil - DateTime.Now).TotalSeconds;
            TimeSpan timeLeft = TimeSpan.FromSeconds(timeLeftDouble);
            if (DateTime.Now < ClaimValidUntil)
            {
                ValidRemaining = string.Format("{0:D2}:{1:D2}:{2:D2}",
                    timeLeft.Hours,
                    timeLeft.Minutes,
                    timeLeft.Seconds);
            }
            else
            {
                ValidTimeRemainingTimer.Enabled = false;
            }
            InvokeAsync(StateHasChanged);
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
            ClaimValidUntil = await _data.AddExtraTimeToClaim();
            this.StateHasChanged();
        }

        protected async Task FinalizeClaim()
        {
            await _data.FinalizeClaim();
            await FinalizeTopic.InvokeAsync();
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