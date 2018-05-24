using Newtonsoft.Json;

namespace Waives.APIClient.Sample.Responses.Document.Classify
{
    internal class ClassifyDocumentResponse
    {
        [JsonProperty("classification_results")]
        public ClassifyDocumentResultsResponse Results;
    }
}