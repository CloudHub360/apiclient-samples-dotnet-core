using Newtonsoft.Json;

namespace CH360.Platform.WebApi.Responses.ExtractData
{
    public class ExtractDataAreaResponse
    {
        [JsonProperty("top")]
        public double Top { get; set; }

        [JsonProperty("left")]
        public double Left { get; set; }

        [JsonProperty("bottom")]
        public double Bottom { get; set; }

        [JsonProperty("right")]
        public double Right { get; set; }

        [JsonProperty("page_number")]
        public int PageNumber { get; set; }
    }
}