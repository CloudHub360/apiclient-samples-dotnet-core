using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Waives.APIClient.Sample.Responses;
using Newtonsoft.Json;

namespace Waives.APIClient.Sample
{
    internal class ApiRequest<TResponse>
    {
        private readonly string _requestUri;
        private readonly HttpClient _httpClient;

        public ApiRequest(string requestUri, HttpClient httpClient)
        {
            if (string.IsNullOrWhiteSpace(requestUri))
            {
                throw new ArgumentNullException(nameof(requestUri));
            }

            _requestUri = requestUri;
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        }

        public async Task<TResponse> Issue(HttpMethod method = null)
        {
            method = method ?? HttpMethod.Get;
            using (var request = new HttpRequestMessage(method, _requestUri))
            using (var response = await _httpClient.SendAsync(request))
            {
                return await HandleResponse<TResponse>(response);
            }
        }

        public async Task<TResponse> Issue<TRequest>(TRequest requestBody, HttpMethod method = null)
        {
            method = method ?? HttpMethod.Post;

            using (var request = new HttpRequestMessage(method, _requestUri) { Content = new JsonContent(requestBody) })
            using (var response = await _httpClient.SendAsync(request))
            {
                return await HandleResponse<TResponse>(response);
            }
        }

        public async Task<TResponse> IssueBinary(StreamContent content, HttpMethod method = null)
        {
            method = method ?? HttpMethod.Post;

            using (var request = new HttpRequestMessage(method, _requestUri) { Content = content })
            using (var response = await _httpClient.SendAsync(request))
            {
                return await HandleResponse<TResponse>(response);
            }
        }
        public async Task<TResponse> IssueForm(List<KeyValuePair<string,string>> values)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Post, _requestUri) { Content = new FormUrlEncodedContent(values) })
            using (var response = await _httpClient.SendAsync(request))
            {
                return await HandleResponse<TResponse>(response);
            }
        }

        private static async Task<T> HandleResponse<T>(HttpResponseMessage response)
        {
            if (response.IsSuccessStatusCode)
            {
                return await ReadResponse<T>(response.Content);
            }

            return await HandleError<T>(response);
        }

        private static async Task<T> HandleError<T>(HttpResponseMessage response)
        {
            var error = await ReadResponse<ErrorResponse>(response.Content);
            throw new ApiException(error.Message, response);
        }

        private static async Task<T> ReadResponse<T>(HttpContent responseContent)
        {
            var responseString = await responseContent.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<T>(responseString);
        }

        private class JsonContent : StringContent
        {
            public JsonContent(object obj) :
                base(JsonConvert.SerializeObject(obj), Encoding.UTF8, (string)"application/json")
            { }
        }
    }
}