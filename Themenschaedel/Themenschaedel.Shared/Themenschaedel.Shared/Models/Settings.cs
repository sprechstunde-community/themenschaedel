using System;
using System.Collections.Generic;
using System.Text;

namespace Themenschaedel.Shared.Models
{
    public enum Platforms
    {
        None,
        Spotify,
        Youtube,
        Apple
    }

    public class Settings
    {
        public Platforms Platform { get; set; }
    }
}
