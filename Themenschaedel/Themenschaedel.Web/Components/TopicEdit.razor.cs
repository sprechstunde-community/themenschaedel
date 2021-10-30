using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Blazored.Toast.Services;
using Microsoft.AspNetCore.Components;
using Themenschaedel.Shared.Props;
using Themenschaedel.Web.Services.Interfaces;

namespace Themenschaedel.Components
{
    public partial class TopicEdit : ComponentBase
    {
        [Parameter] [Required] public int episodeId { get; set; }
        [Parameter] public Episode episode { get; set; }

        [Inject] protected IData _data { get; set; }
        [Inject] protected IToastService _toastService { get; set; }

        protected GetTopicWorkaroundWrapper wrapper;
        protected List<Topic> localTopics = new List<Topic>();
        protected List<Topic> serverTopics = new List<Topic>();

        protected override async Task OnInitializedAsync()
        {
            await PopulateTopics();
        }

        protected async Task AddTopic()
        {
            localTopics.Add(new Topic());
            this.StateHasChanged();
        }


        protected async Task RemoveTopic(Topic topic)
        {
            if (topic.id != null && topic.id != 0)
            {
                //Server request (delete topic)
                //else is not needed, as else the api never knew about this topic
                await _data.DeleteTopic(topic);
            }

            localTopics.Remove(topic);
            this.StateHasChanged();
            _toastService.ShowSuccess($"Delted Topic: {topic.name}.");
        }

        protected async Task RemoveSubtopic(Subtopics subtopic)
        {
            if (subtopic.id != null && subtopic.id != 0)
            {
                //Server request (delete topic)
                //else is not needed, as else the api never knew about this topic
                await _data.DeleteSubtopic(subtopic);
            }

            this.StateHasChanged();
            _toastService.ShowSuccess($"Delted Topic: {subtopic.name}.");
        }


        protected async Task PopulateTopics()
        {
            localTopics = episode.topics;
            wrapper = await this._data.GetTopics(episodeId);
            if (wrapper.data != null)
            {
                localTopics = wrapper.data;
            }
        }

        protected async Task SendTopic(Topic topic)
        {
            int topicIndex = localTopics.FindIndex(x => x == topic);
            Topic tempTopic = localTopics[topicIndex];

            //Server request
            if (topic.id != null && topic.id != 0)
            {
                //Server request (delete topic)
                //else is not needed, as else the api never knew about this topic
                localTopics[topicIndex] = await _data.UpdateTopic(topic);
            }
            else
            {
                localTopics[topicIndex] = await _data.AddTopic(topic, episodeId);
            }

            if (localTopics[topicIndex] != null)
            {
                _toastService.ShowSuccess($"Saved Topic: {tempTopic.name}.");
            }
        }

        protected async Task SendTopics()
        {
            // Server request
            for (int i = 0; i < localTopics.Count; i++)
            {
                await SendTopic(localTopics[i]);
            }
        }
    }
}