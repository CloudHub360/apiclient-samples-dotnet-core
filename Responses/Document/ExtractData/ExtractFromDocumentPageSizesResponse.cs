using System.Collections.Generic;
using Newtonsoft.Json;

namespace CH360.Platform.WebApi.Responses.ExtractData
{
    public class ExtractDataPageSizesResponse
    {
        [JsonProperty("page_count")]
        public int PageCount { get; set; }

        [JsonProperty("pages")]
        public List<ExtractDataPageSizeResponse> Pages { get; set; }
    }
}