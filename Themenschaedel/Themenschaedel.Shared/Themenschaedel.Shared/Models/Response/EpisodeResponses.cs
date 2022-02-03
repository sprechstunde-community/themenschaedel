using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Themenschaedel.Shared.Models.Response
{
    public class EpisodeResponse
    {
        [JsonPropertyName("data")]
        public List<EpisodeExtended> Data { get; set; } = new List<EpisodeExtended>();

        [JsonPropertyName("meta")] public Meta Meta { get; set; } = new Meta();
    }

    public class EpisodeAlternateResponse
    {
        [JsonPropertyName("data")]
        public List<EpisodeExtendedExtra> Data { get; set; } = new List<EpisodeExtendedExtra>();

        [JsonPropertyName("meta")] public Meta Meta { get; set; } = new Meta();
    }

    public class Meta
    {
        [JsonPropertyName("total")]
        public int EpisodeCount { get; set; }
        [JsonPropertyName("last_page")]
        public int EpisodeMaxPageCount { get; set; }
    }
}
