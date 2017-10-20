using Newtonsoft.Json;

namespace CH360.APIClient.Sample.Responses.Document
{
    internal class DocumentFileResponse
    {
        public string ID { get; set; }

        [JsonProperty("file_type")]
        public string FileType { get; set; }

        public int Size { get; set; }

        public string SHA256 { get; set; }
    }
}