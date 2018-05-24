using System.Collections.Generic;
using Newtonsoft.Json;

namespace Waives.APIClient.Sample.Responses.Document.Classify
{
    internal class ClassifyDocumentResultsResponse
    {
        [JsonProperty("document_type")]
        public string DocumentType;

        [JsonProperty("is_confident")]
        public bool IsConfident;

        [JsonProperty("document_type_scores")]
        public List<ClassifyDocumentScoresResponse> Scores;
    }
}