using System;
using System.Collections.Generic;
using System.Text;

namespace Themenschaedel.Shared.Props
{
    public class GetEpisodesWorkaround
    {
        public List<Episode> data { get; set; }
        public WorkaroundMeta meta { get; set; }
    }

    public class WorkaroundMeta
    {
        public int last_page { get; set; }
        public int current_page { get; set; }
    }
}
