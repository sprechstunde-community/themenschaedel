using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using Themenschaedel.Shared.Models;
using Themenschaedel.Web.Services.Interfaces;
using Humanizer;

namespace Themenschaedel.Components
{
    public partial class TopicView : ComponentBase
    {
        [Parameter] [Required] public int episodeId { get; set; }
        [Parameter] public EpisodeClientExtra episode { get; set; }

        [Inject] private IRefresher _refresh { get; set; }

        [Inject] protected IData _data { get; set; }
        
        protected List<TopicExtended> topics = new List<TopicExtended>();

        protected override void OnInitialized()
        {
            _refresh.Refresh += (sender, args) =>
            {
                topics = episode.Topic;
            };
            topics = episode.Topic;
        }
    }
}