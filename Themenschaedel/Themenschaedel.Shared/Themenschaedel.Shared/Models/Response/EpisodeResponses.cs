using System;
using System.Collections.Generic;
using System.Text;

namespace Themenschaedel.Shared.Models.Response
{
    public class EpisodeResponse
    {
        public List<Episode> data { get; set; } = new List<Episode>();
        public Meta Meta { get; set; }
    }

    public class Meta
    {

    }
}
