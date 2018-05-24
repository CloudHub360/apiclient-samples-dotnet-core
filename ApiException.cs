using System;
using System.Net.Http;

namespace Waives.APIClient.Sample
{
    public class ApiException : Exception
    {
        public ApiException()
        {
        }

        public ApiException(string message) : base(message)
        {
        }

        public ApiException(string message, HttpResponseMessage response) : this(
            $"{(int)response.StatusCode} ({response.ReasonPhrase}): {message}")
        {
        }

        public ApiException(HttpResponseMessage response) : this(
            $"Unexpected HTTP response: {(int)response.StatusCode} ({response.ReasonPhrase})")
        {
        }

        public ApiException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}