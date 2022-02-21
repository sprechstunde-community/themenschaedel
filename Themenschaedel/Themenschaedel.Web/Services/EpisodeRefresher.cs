using System;
using System.Threading.Tasks;
using Themenschaedel.Web.Services.Interfaces;

namespace Themenschaedel.Web.Services
{
    public class EpisodeRefresher : IRefresher
    {
        public event EventHandler Refresh;

        public void FireRefresh()
        {
            Refresh?.Invoke(null, null);
        }
    }
}
