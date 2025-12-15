using System.Net.Http;

namespace ShippingRecorder.Tests.Mocks
{
    internal class MockHttpRequest
    {
        public HttpMethod Method { get; set; }
        public string Uri { get; set; }
        public HttpContent Content { get; set; }
    }
}