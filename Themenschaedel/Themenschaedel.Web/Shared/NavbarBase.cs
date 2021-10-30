using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using Themenschaedel.Components.Controls;

namespace Themenschaedel.Web.Shared
{
    public partial class NavbarBase : ComponentBase
    {
        [Inject] private ILogger<NavbarBase> _logger { get; set; }

        public bool CheckedSentry = true;
        
        public void OnSentryClick()
        {
            _logger.LogDebug($"LEBUTTON!!! {CheckedSentry}");
        }
    }
}