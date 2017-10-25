using System.Collections.Generic;
using Newtonsoft.Json;

namespace CH360.Platform.WebApi.Responses.ExtractData
{
    public class ExtractDataResponse
    {
        [JsonProperty("field_results")]
        public List<ExtractDataFieldResultResponse> FieldResults { get; set; }

        [JsonProperty("page_sizes")]
        public ExtractDataPageSizesResponse PageSizes { get; set; }
    }
}