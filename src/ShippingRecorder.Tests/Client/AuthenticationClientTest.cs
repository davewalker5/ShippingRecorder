using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using ShippingRecorder.Client.ApiClient;
using ShippingRecorder.Client.Interfaces;
using ShippingRecorder.Entities.Config;
using ShippingRecorder.Tests.Mocks;

namespace ShippingRecorder.Tests.Client
{
    [TestClass]
    public class AuthenticationClientTest
    {
        private string _token;
        private readonly MockShippingRecorderHttpClient _httpClient = new();
        private IAuthenticationClient _client;

        private readonly ShippingRecorderApplicationSettings _settings = new()
        {
            ApiUrl = "http://server/",
            ApiRoutes = [
                new() { Name = "Authenticate", Route = "/users/authenticate" }
            ]
        };

        [TestInitialize]
        public void Initialise()
        {
            _token = DataGenerator.RandomWord(200, 200);
            var provider = new Mock<IAuthenticationTokenProvider>();
            provider.Setup(x => x.GetToken()).Returns(_token);
            var logger = new Mock<ILogger<AuthenticationClient>>();
            var cache = new Mock<ICacheWrapper>();
            _client = new AuthenticationClient(_httpClient, _settings, provider.Object, cache.Object, logger.Object);
        }

        [TestMethod]
        public async Task AuthenticateTest()
        {
            _httpClient.AddResponse(_token);

            var token = await _client.AuthenticateAsync("", "");

            Assert.AreEqual($"{_settings.ApiUrl}", _httpClient.BaseAddress.ToString());
            Assert.AreEqual(HttpMethod.Post, _httpClient.Requests[0].Method);
            Assert.AreEqual(_settings.ApiRoutes[0].Route, _httpClient.Requests[0].Uri);

            Assert.AreEqual(token, _token);
        }
    }
}