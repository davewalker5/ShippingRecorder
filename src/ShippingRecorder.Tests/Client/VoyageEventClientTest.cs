using System.Text.Json;
using ShippingRecorder.Client.Interfaces;
using ShippingRecorder.Tests.Mocks;
using Microsoft.Extensions.Logging;
using Moq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ShippingRecorder.Entities.Config;
using ShippingRecorder.Client.ApiClient;
using System.Threading.Tasks;
using System.Net.Http;
using System.Collections.Generic;
using ShippingRecorder.Entities.Db;

namespace ShippingRecorder.Tests.Client
{
    [TestClass]
    public class VoyageEventClientTest
    {
        private readonly string ApiToken = "An API Token";
        private readonly MockShippingRecorderHttpClient _httpClient = new();
        private IVoyageEventClient _client;

        private readonly ShippingRecorderApplicationSettings _settings = new()
        {
            ApiUrl = "http://server/",
            ApiRoutes = [
                new() { Name = "VoyageEvent", Route = "/voyageevents" }
            ]
        };

        [TestInitialize]
        public void Initialise()
        {
            var provider = new Mock<IAuthenticationTokenProvider>();
            provider.Setup(x => x.GetToken()).Returns(ApiToken);
            var logger = new Mock<ILogger<VoyageEventClient>>();
            var cache = new Mock<ICacheWrapper>();
            _client = new VoyageEventClient(_httpClient, _settings, provider.Object, cache.Object, logger.Object);
        }

        [TestMethod]
        public async Task AddTest()
        {
            var voyageEvent = DataGenerator.CreateVoyageEvent();
            var json = JsonSerializer.Serialize(
                new
                {
                    voyageEvent.VoyageId,
                    voyageEvent.EventType,
                    voyageEvent.PortId,
                    voyageEvent.Date
                });
            _httpClient.AddResponse(json);

            var added = await _client.AddAsync(voyageEvent.VoyageId, voyageEvent.EventType, voyageEvent.PortId, voyageEvent.Date);

            Assert.AreEqual($"Bearer {ApiToken}", _httpClient.DefaultRequestHeaders.Authorization.ToString());
            Assert.AreEqual($"{_settings.ApiUrl}", _httpClient.BaseAddress.ToString());
            Assert.AreEqual(HttpMethod.Post, _httpClient.Requests[0].Method);
            Assert.AreEqual(_settings.ApiRoutes[0].Route, _httpClient.Requests[0].Uri);

            Assert.AreEqual(json, await _httpClient.Requests[0].Content.ReadAsStringAsync());
            Assert.IsNotNull(added);
            Assert.AreEqual(voyageEvent.VoyageId, added.VoyageId);
            Assert.AreEqual(voyageEvent.EventType, added.EventType);
            Assert.AreEqual(voyageEvent.PortId, added.PortId);
            Assert.AreEqual(voyageEvent.Date, added.Date);
        }

        [TestMethod]
        public async Task UpdateTest()
        {
            var voyageEvent = DataGenerator.CreateVoyageEvent();
            var json = JsonSerializer.Serialize(voyageEvent);
            _httpClient.AddResponse(json);

            var updated = await _client.UpdateAsync(voyageEvent.Id, voyageEvent.VoyageId, voyageEvent.EventType, voyageEvent.PortId, voyageEvent.Date);

            Assert.AreEqual($"Bearer {ApiToken}", _httpClient.DefaultRequestHeaders.Authorization.ToString());
            Assert.AreEqual($"{_settings.ApiUrl}", _httpClient.BaseAddress.ToString());
            Assert.AreEqual(HttpMethod.Put, _httpClient.Requests[0].Method);
            Assert.AreEqual(_settings.ApiRoutes[0].Route, _httpClient.Requests[0].Uri);

            Assert.StartsWith((await _httpClient.Requests[0].Content.ReadAsStringAsync())[..^1], json);
            Assert.IsNotNull(updated);
            Assert.AreEqual(voyageEvent.Id, updated.Id);
            Assert.AreEqual(voyageEvent.VoyageId, updated.VoyageId);
            Assert.AreEqual(voyageEvent.EventType, updated.EventType);
            Assert.AreEqual(voyageEvent.PortId, updated.PortId);
            Assert.AreEqual(voyageEvent.Date, updated.Date);
        }

        [TestMethod]
        public async Task DeleteTest()
        {
            var id = DataGenerator.RandomId();
            await _client.DeleteAsync(id);

            Assert.AreEqual($"Bearer {ApiToken}", _httpClient.DefaultRequestHeaders.Authorization.ToString());
            Assert.AreEqual($"{_settings.ApiUrl}", _httpClient.BaseAddress.ToString());
            Assert.AreEqual(HttpMethod.Delete, _httpClient.Requests[0].Method);
            Assert.AreEqual($"{_settings.ApiRoutes[0].Route}/{id}", _httpClient.Requests[0].Uri);

            Assert.IsNull(_httpClient.Requests[0].Content);
        }

        [TestMethod]
        public async Task ListTest()
        {
            var voyageEvent = DataGenerator.CreateVoyageEvent();
            var json = JsonSerializer.Serialize<List<VoyageEvent>>([voyageEvent]);
            _httpClient.AddResponse(json);

            var voyageEvents = await _client.ListAsync(voyageEvent.VoyageId, 1, int.MaxValue);
            var expectedRoute = $"{_settings.ApiRoutes[0].Route}/{voyageEvent.VoyageId}/1/{int.MaxValue}";

            Assert.AreEqual($"Bearer {ApiToken}", _httpClient.DefaultRequestHeaders.Authorization.ToString());
            Assert.AreEqual($"{_settings.ApiUrl}", _httpClient.BaseAddress.ToString());
            Assert.AreEqual(HttpMethod.Get, _httpClient.Requests[0].Method);
            Assert.AreEqual(expectedRoute, _httpClient.Requests[0].Uri);

            Assert.IsNull(_httpClient.Requests[0].Content);
            Assert.IsNotNull(voyageEvents);
            Assert.HasCount(1, voyageEvents);
            Assert.AreEqual(voyageEvent.Id, voyageEvents[0].Id);
            Assert.AreEqual(voyageEvent.VoyageId, voyageEvents[0].VoyageId);
            Assert.AreEqual(voyageEvent.EventType, voyageEvents[0].EventType);
            Assert.AreEqual(voyageEvent.PortId, voyageEvents[0].PortId);
            Assert.AreEqual(voyageEvent.Date, voyageEvents[0].Date);
        }
    }
}
