using Newtonsoft.Json;

namespace CH360.APIClient.Sample.Responses.Document
{
    internal class DocumentResponse
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("_embedded")]
        public DocumentEmbeddedResourcesResponse EmbeddedResources { get; set; }
    }
}