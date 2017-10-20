using Newtonsoft.Json;

namespace CH360.APIClient.Sample.Responses.Document.Classify
{
    internal class ClassifyDocumentScoresResponse
    {
        [JsonProperty("document_type")]
        public string DocumentType;

        [JsonProperty("score")]
        public double Score;
    }
}