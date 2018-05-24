using Newtonsoft.Json;

namespace Waives.APIClient.Sample.Requests
{
    internal class GetTokenRequest
    {
        [JsonProperty("client_id")]
        public string ClientId { get; set; }
        [JsonProperty("client_secret")]
        public string ClientSecret { get; set; }
    }
}