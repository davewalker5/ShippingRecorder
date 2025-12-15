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
    public class VesselClientTest
    {
        private readonly string ApiToken = "An API Token";
        private readonly MockShippingRecorderHttpClient _httpClient = new();
        private IVesselClient _client;

        private readonly ShippingRecorderApplicationSettings _settings = new()
        {
            ApiUrl = "http://server/",
            ApiRoutes = [
                new() { Name = "Vessel", Route = "/vessels" }
            ]
        };

        [TestInitialize]
        public void Initialise()
        {
            var provider = new Mock<IAuthenticationTokenProvider>();
            provider.Setup(x => x.GetToken()).Returns(ApiToken);
            var logger = new Mock<ILogger<VesselClient>>();
            _client = new VesselClient(_httpClient, _settings, provider.Object, logger.Object);
        }

        [TestMethod]
        public async Task AddTest()
        {
            var vessel = DataGenerator.CreateVessel();
            var json = JsonSerializer.Serialize(new
            {
                vessel.IMO,
                vessel.Built,
                vessel.Draught,
                vessel.Length,
                vessel.Beam
            });
            _httpClient.AddResponse(json);

            var added = await _client.AddAsync(vessel.IMO, vessel.Built, vessel.Draught, vessel.Length, vessel.Beam);

            Assert.AreEqual($"Bearer {ApiToken}", _httpClient.DefaultRequestHeaders.Authorization.ToString());
            Assert.AreEqual($"{_settings.ApiUrl}", _httpClient.BaseAddress.ToString());
            Assert.AreEqual(HttpMethod.Post, _httpClient.Requests[0].Method);
            Assert.AreEqual(_settings.ApiRoutes[0].Route, _httpClient.Requests[0].Uri);

            Assert.AreEqual(json, await _httpClient.Requests[0].Content.ReadAsStringAsync());
            Assert.IsNotNull(added);
            Assert.AreEqual(vessel.IMO, added.IMO);
            Assert.AreEqual(vessel.Built, added.Built);
            Assert.AreEqual(vessel.Draught, added.Draught);
            Assert.AreEqual(vessel.Length, added.Length);
            Assert.AreEqual(vessel.Beam, added.Beam);
        }

        [TestMethod]
        public async Task UpdateTest()
        {
            var vessel = DataGenerator.CreateVessel();
            var json = JsonSerializer.Serialize(vessel);
            _httpClient.AddResponse(json);

            var updated = await _client.UpdateAsync(vessel.Id, vessel.IMO, vessel.Built, vessel.Draught, vessel.Length, vessel.Beam);

            Assert.AreEqual($"Bearer {ApiToken}", _httpClient.DefaultRequestHeaders.Authorization.ToString());
            Assert.AreEqual($"{_settings.ApiUrl}", _httpClient.BaseAddress.ToString());
            Assert.AreEqual(HttpMethod.Put, _httpClient.Requests[0].Method);
            Assert.AreEqual(_settings.ApiRoutes[0].Route, _httpClient.Requests[0].Uri);

            Assert.StartsWith((await _httpClient.Requests[0].Content.ReadAsStringAsync())[..^1], json);
            Assert.IsNotNull(updated);
            Assert.AreEqual(vessel.Id, updated.Id);
            Assert.AreEqual(vessel.IMO, updated.IMO);
            Assert.AreEqual(vessel.Built, updated.Built);
            Assert.AreEqual(vessel.Draught, updated.Draught);
            Assert.AreEqual(vessel.Length, updated.Length);
            Assert.AreEqual(vessel.Beam, updated.Beam);
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
            var vessel = DataGenerator.CreateVessel();
            var json = JsonSerializer.Serialize<List<Vessel>>([vessel]);
            _httpClient.AddResponse(json);

            var vessels = await _client.ListAsync(1, int.MaxValue);
            var expectedRoute = $"{_settings.ApiRoutes[0].Route}/1/{int.MaxValue}";

            Assert.AreEqual($"Bearer {ApiToken}", _httpClient.DefaultRequestHeaders.Authorization.ToString());
            Assert.AreEqual($"{_settings.ApiUrl}", _httpClient.BaseAddress.ToString());
            Assert.AreEqual(HttpMethod.Get, _httpClient.Requests[0].Method);
            Assert.AreEqual(expectedRoute, _httpClient.Requests[0].Uri);

            Assert.IsNull(_httpClient.Requests[0].Content);
            Assert.IsNotNull(vessels);
            Assert.HasCount(1, vessels);
            Assert.AreEqual(vessel.Id, vessels[0].Id);
            Assert.AreEqual(vessel.IMO, vessels[0].IMO);
            Assert.AreEqual(vessel.Built, vessels[0].Built);
            Assert.AreEqual(vessel.Draught, vessels[0].Draught);
            Assert.AreEqual(vessel.Length, vessels[0].Length);
            Assert.AreEqual(vessel.Beam, vessels[0].Beam);
        }
    }
}
