using System.Collections.Generic;
using Newtonsoft.Json;

namespace CH360.APIClient.Sample.Responses.Document.ExtractData
{
    public class ExtractDataResponse
    {
        [JsonProperty("field_results")]
        public List<ExtractDataFieldResultResponse> FieldResults { get; set; }

        [JsonProperty("page_sizes")]
        public ExtractDataPageSizesResponse PageSizes { get; set; }
    }
}