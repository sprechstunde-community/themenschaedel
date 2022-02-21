using System;
using Themenschaedel.Web.Services.Interfaces;

namespace Themenschaedel.Web.Services
{
    public class EpisodeCollectionRefresher : IRefresher
    {
        public event EventHandler Refresh;

        public void FireRefresh()
        {
            Refresh?.Invoke(null, null);
        }
    }
}
