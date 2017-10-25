using Newtonsoft.Json;

namespace CH360.Platform.WebApi.Responses.ExtractData
{
    public class ExtractDataFieldResultResponse
    {
        [JsonProperty("field_name")]
        public string FieldName { get; set; }

        [JsonProperty("result")]
        public ExtractDataResultResponse Result { get; set; }

        [JsonProperty("alternatives")]
        public object Alternatives { get; set; }

        [JsonProperty("tabular_results")]
        public object TabularResults { get; set; }

        [JsonProperty("rejected")]
        public bool Rejected { get; set; }

        [JsonProperty("reject_reason")]
        public string RejectReason { get; set; }
    }
}