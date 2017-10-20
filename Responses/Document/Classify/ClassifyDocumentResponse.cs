using Newtonsoft.Json;

namespace CH360.APIClient.Sample.Responses.Document.Classify
{
    internal class ClassifyDocumentResponse
    {
        [JsonProperty("classification_results")]
        public ClassifyDocumentResultsResponse Results;
    }
}