using System.Collections.Generic;
using Newtonsoft.Json;

namespace CH360.APIClient.Sample.Responses.Classifier
{
    internal class ClassifierAddSamplesResponse
    {
        [JsonProperty("samples")]
        public List<SampleResponse> Samples { get; set; }
    }
}