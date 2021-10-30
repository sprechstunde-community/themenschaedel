using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using Themenschaedel.Shared.Props;
using Themenschaedel.Web.Services.Interfaces;
using Humanizer;

namespace Themenschaedel.Components
{
    public partial class TopicView : ComponentBase
    {
        [Parameter] [Required] public int episodeId { get; set; }
        [Parameter] public Episode episode { get; set; }

        [Inject] protected IData _data { get; set; }

        protected GetTopicWorkaroundWrapper wrapper;
        protected List<Topic> topics = new List<Topic>();

        protected override async Task OnInitializedAsync()
        {
            topics = episode.topics;
            wrapper = await this._data.GetTopics(episodeId);
            if (wrapper.data != null)
            {
                topics = wrapper.data;
            }
        }
    }
}