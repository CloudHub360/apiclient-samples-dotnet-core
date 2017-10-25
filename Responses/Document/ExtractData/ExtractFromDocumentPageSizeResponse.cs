using Newtonsoft.Json;

namespace CH360.Platform.WebApi.Responses.ExtractData
{
    public class ExtractDataPageSizeResponse
    {
        [JsonProperty("page_number")]
        public int PageNumber { get; set; }

        [JsonProperty("width")]
        public double Width { get; set; }

        [JsonProperty("height")]
        public double Height { get; set; }
    }
}