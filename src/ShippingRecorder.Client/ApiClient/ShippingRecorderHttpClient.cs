using System;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using ShippingRecorder.Client.Interfaces;

namespace ShippingRecorder.Client.ApiClient
{
    [ExcludeFromCodeCoverage]
    public sealed class ShippingRecorderHttpClient : IShippingRecorderHttpClient
    {
        private readonly static HttpClient _client = new();
        private static ShippingRecorderHttpClient _instance = null;
        private readonly static object _lock = new();

        private ShippingRecorderHttpClient() { }

        public Uri BaseAddress
        {
            get { return _client.BaseAddress;}
            set
            {
                if (_client.BaseAddress == null)
                {
                    _client.BaseAddress = value;
                }
            }
        }

        public HttpRequestHeaders DefaultRequestHeaders
        {
            get { return _client.DefaultRequestHeaders; }
        }

        /// <summary>
        /// Return the singleton instance of the client
        /// </summary>
        public static ShippingRecorderHttpClient Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        _instance ??= new ShippingRecorderHttpClient();
                    }
                }

                return _instance;
            }
        }

        /// <summary>
        /// Send a GET request to the specified URI and return the response
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        public Task<HttpResponseMessage> GetAsync(string uri)
            => _client.GetAsync(uri);

        /// <summary>
        /// POST data to the specified URI
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        public Task<HttpResponseMessage> PostAsync(string uri, HttpContent content)
            => _client.PostAsync(uri, content);

        /// <summary>
        /// Send a PUT request to update the resource at the specified URI
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        public Task<HttpResponseMessage> PutAsync(string uri, HttpContent content)
            => _client.PutAsync(uri, content);

        public Task<HttpResponseMessage> DeleteAsync(string uri)
            => _client.DeleteAsync(uri);
    }
}