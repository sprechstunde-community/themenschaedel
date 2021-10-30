using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

namespace Themenschaedel.Components.Controls
{
    public class CheckControl : ComponentBase
    {
        [Required] [Parameter] public string Title { get; set; }

        [Parameter] public bool IsChecked { get; set; } = false;
        
        [Parameter] public EventCallback<bool> OnClick { get; set; }

        public void OnClickAction()
        {
            
        }
        
        protected async void OnClickInternal()
        {
            IsChecked = !IsChecked;
            await OnClick.InvokeAsync(IsChecked);
        }

    }
}
