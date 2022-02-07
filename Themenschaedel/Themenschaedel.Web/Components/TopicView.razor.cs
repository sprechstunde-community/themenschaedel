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

        [Inject] protected IData _data { get; set; }
        
        protected List<TopicExtended> topics = new List<TopicExtended>();

        protected override async Task OnInitializedAsync()
        {
            topics = episode.Topic;
        }
    }
}