using System.Collections.Generic;
using CH360.APIClient.Sample.Responses.Document.ExtractData;
using Newtonsoft.Json;

namespace CH360.APIClient.Sample.Responses.Document.ExtractData
{
    public class ExtractDataResultResponse
    {
        [JsonProperty("text")]
        public string Text { get; set; }

        [JsonProperty("value")]
        public string Value { get; set; }

        [JsonProperty("rejected")]
        public bool Rejected { get; set; }

        [JsonProperty("reject_reason")]
        public string RejectReason { get; set; }

        [JsonProperty("areas")]
        public List<ExtractDataAreaResponse> Areas { get; set; }

        [JsonProperty("proximity_score")]
        public double ProximityScore { get; set; }

        [JsonProperty("match_score")]
        public double MatchScore { get; set; }

        [JsonProperty("text_score")]
        public double TextScore { get; set; }
    }
}