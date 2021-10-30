using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Themenschaedel.Shared.Props;
using Themenschaedel.Web.Services.Interfaces;

namespace Themenschaedel.Components
{
    public partial class EpisodeCollection : ComponentBase
    {
        [Inject] private IJSRuntime JSRuntime { get; set; }
        [Inject] private IData Data { get; set; }

        public bool IsLoading { get; set; } = false;

        public int PageSize = 32;

        public int PageNumber = 1;

        public bool StopLoading = false;

        public List<Episode> Episodes { get; set; } = new List<Episode>();

        Random random = new Random();

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await LoadMore();
                await InitJsListenerAsync();
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

                GetEpisodesWorkaround ep = await Data.GetEpisodes(PageSize, PageNumber);
                for (int j = 0; j < ep.data.Count; j++)
                {
                    ep.data[j].AnimationDelay = _cssDelay + j * 100;

                    Episodes.Add(ep.data[j]);

                    if (Episodes[Episodes.Count - 1].image == null || Episodes[Episodes.Count - 1].image == "")
                    {
                        Episodes[Episodes.Count - 1].image = "assets/WhiteThemenschaedel.png";
                        if (random.Next(1, 10001) > 9995)
                        {
                            Episodes[Episodes.Count - 1].image = "assets/WhiteThemenschaedel3.png";
                        }
                        if (random.Next(1, 10001) > 9900)
                        {
                            Episodes[Episodes.Count - 1].image = "assets/WhiteThemenschaedel2.png";
                        }
                    }
                }

                PageNumber++;

                IsLoading = false;

                StateHasChanged();


                //at the end of pages or results stop loading anymore
                if (PageNumber > ep.meta.last_page)
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
