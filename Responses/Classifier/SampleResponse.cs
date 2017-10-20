using Newtonsoft.Json;

namespace CH360.APIClient.Sample.Responses.Classifier
{
    internal class SampleResponse
    {
        [JsonProperty("path")]
        public string Path { get; set; }

        [JsonProperty("document_type")]
        public string DocumentType { get; set; }
    }
}