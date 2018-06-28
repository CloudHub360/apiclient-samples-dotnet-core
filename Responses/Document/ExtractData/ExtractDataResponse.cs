using System.Collections.Generic;
using Newtonsoft.Json;

namespace Waives.APIClient.Sample.Responses.Document.ExtractData
{
    public class ExtractDataResponse
    {
        [JsonProperty("field_results")]
        public List<ExtractDataFieldResultResponse> FieldResults { get; set; }

        [JsonProperty("document")]
        public ExtractDataDocumentResponse Document { get; set; }
    }
}