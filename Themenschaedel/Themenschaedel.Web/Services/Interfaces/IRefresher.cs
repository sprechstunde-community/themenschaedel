using System;

namespace Themenschaedel.Web.Services.Interfaces
{
    public interface IRefresher
    {
        event EventHandler Refresh;
        void FireRefresh();
    }
}
