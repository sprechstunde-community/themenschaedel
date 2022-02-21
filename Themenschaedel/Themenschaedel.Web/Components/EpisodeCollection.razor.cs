using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Themenschaedel.Shared.Models;
using Themenschaedel.Shared.Models.Response;
using Themenschaedel.Web.Services.Interfaces;

namespace Themenschaedel.Components
{
    public partial class EpisodeCollection : ComponentBase
    {
        [Inject] private IJSRuntime JSRuntime { get; set; }
        [Inject] private IData _data { get; set; }
        [Inject] private IUserSession _session { get; set; }

        public bool IsLoading { get; set; } = false;

        public int PageSize = 32;

        public int PageNumber = 1;

        public bool StopLoading = false;

        public List<EpisodeClient> Episodes { get; set; } = new List<EpisodeClient>();

        Random random = new Random();

        public int LastSeenEpisodeNumber = 0;

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await PopulateLastSeenEpisodeNumber();
                await LoadMore();
                await InitJsListenerAsync();
                await SetLastEpisodeNumber();
            }
        }

        private async Task PopulateLastSeenEpisodeNumber()
        {
            LastSeenEpisodeNumber = await _session.GetLastSeenEpisodeNumber();
        }

        protected async Task SetLastEpisodeNumber()
        {
            if (Episodes.Count != 0)
            {
                await _session.SetLastSeenEpisodeNumber(Episodes[0].EpisodeNumber);
            }
        }

        protected async Task InitJsListenerAsync()
        {
            await JSRuntime.InvokeVoidAsync("ScrollList.Init", "list-end", DotNetObjectReference.Create(this));
        }

        [JSInvokable]
        public async Task LoadMore()
        {
            if (!IsLoading)
            {
                IsLoading = true;

                StateHasChanged();

                //await Task.Delay(1000);
                EpisodeResponse ep = await _data.GetEpisodes(PageSize, PageNumber);
                if (ep == null) return;
                for (int j = 0; j < ep.Data.Count; j++)
                {
                    EpisodeClient episodeClient = new EpisodeClient(ep.Data[j]);
                    episodeClient.AnimationDelay = _cssDelay + j * 100;

                    Episodes.Add(episodeClient);

                    if (Episodes[Episodes.Count - 1].Image == null || Episodes[Episodes.Count - 1].Image == "")
                    {
                        Episodes[Episodes.Count - 1].Image = "assets/WhiteThemenschaedel.png";
                        if (random.Next(1, 10001) > 9995)
                        {
                            Episodes[Episodes.Count - 1].Image = "assets/WhiteThemenschaedel3.png";
                        }
                        if (random.Next(1, 10001) > 9900)
                        {
                            Episodes[Episodes.Count - 1].Image = "assets/WhiteThemenschaedel2.png";
                        }
                    }
                }

                PageNumber++;

                IsLoading = false;

                StateHasChanged();


                //at the end of pages or results stop loading anymore
                if (PageNumber > ep.Meta.EpisodeMaxPageCount)
                {
                    await StopListener();
                }
            }
        }

        public async Task StopListener()
        {
            StopLoading = true;
            IsLoading = false;
            await JSRuntime.InvokeVoidAsync("ScrollList.RemoveListener");
            StateHasChanged();
        }


        public void Dispose()
        {
            JSRuntime.InvokeVoidAsync("ScrollList.RemoveListener");
        }



        #region FrontEnd


        private const int _cssDelay = 0;

        #endregion FrontEnd
    }
}
