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
using System;

namespace ShippingRecorder.Tests.Client
{
    [TestClass]
    public class PortClientTest
    {
        private readonly string ApiToken = "An API Token";
        private readonly MockShippingRecorderHttpClient _httpClient = new();
        private IPortClient _client;

        private readonly ShippingRecorderApplicationSettings _settings = new()
        {
            ApiUrl = "http://server/",
            ApiRoutes = [
                new() { Name = "Port", Route = "/ports" }
            ]
        };

        [TestInitialize]
        public void Initialise()
        {
            var provider = new Mock<IAuthenticationTokenProvider>();
            provider.Setup(x => x.GetToken()).Returns(ApiToken);
            var logger = new Mock<ILogger<PortClient>>();
            _client = new PortClient(_httpClient, _settings, provider.Object, logger.Object);
        }

        [TestMethod]
        public async Task AddTest()
        {
            var port = DataGenerator.CreatePort();
            var json = JsonSerializer.Serialize(new { port.CountryId, port.Code, port.Name });
            _httpClient.AddResponse(json);

            var added = await _client.AddAsync(port.CountryId, port.Code, port.Name);

            Assert.AreEqual($"Bearer {ApiToken}", _httpClient.DefaultRequestHeaders.Authorization.ToString());
            Assert.AreEqual($"{_settings.ApiUrl}", _httpClient.BaseAddress.ToString());
            Assert.AreEqual(HttpMethod.Post, _httpClient.Requests[0].Method);
            Assert.AreEqual(_settings.ApiRoutes[0].Route, _httpClient.Requests[0].Uri);

            Assert.StartsWith((await _httpClient.Requests[0].Content.ReadAsStringAsync())[..^1], json);
            Assert.IsNotNull(added);
            Assert.AreEqual(port.CountryId, added.CountryId);
            Assert.AreEqual(port.Code, added.Code);
            Assert.AreEqual(port.Name, added.Name);
        }

        [TestMethod]
        public async Task UpdateTest()
        {
            var port = DataGenerator.CreatePort();
            var json = JsonSerializer.Serialize(port);
            _httpClient.AddResponse(json);

            var updated = await _client.UpdateAsync(port.Id, port.CountryId, port.Code, port.Name);

            Assert.AreEqual($"Bearer {ApiToken}", _httpClient.DefaultRequestHeaders.Authorization.ToString());
            Assert.AreEqual($"{_settings.ApiUrl}", _httpClient.BaseAddress.ToString());
            Assert.AreEqual(HttpMethod.Put, _httpClient.Requests[0].Method);
            Assert.AreEqual(_settings.ApiRoutes[0].Route, _httpClient.Requests[0].Uri);

            Assert.StartsWith((await _httpClient.Requests[0].Content.ReadAsStringAsync())[..^1], json);
            Assert.IsNotNull(updated);
            Assert.AreEqual(port.Id, updated.Id);
            Assert.AreEqual(port.CountryId, updated.CountryId);
            Assert.AreEqual(port.Code, updated.Code);
            Assert.AreEqual(port.Name, updated.Name);
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
            var port = DataGenerator.CreatePort();
            var json = JsonSerializer.Serialize<List<Port>>([port]);
            _httpClient.AddResponse(json);

            var ports = await _client.ListAsync(port.CountryId, 1, int.MaxValue);
            var expectedRoute = $"{_settings.ApiRoutes[0].Route}/{port.CountryId}/1/{int.MaxValue}";

            Assert.AreEqual($"Bearer {ApiToken}", _httpClient.DefaultRequestHeaders.Authorization.ToString());
            Assert.AreEqual($"{_settings.ApiUrl}", _httpClient.BaseAddress.ToString());
            Assert.AreEqual(HttpMethod.Get, _httpClient.Requests[0].Method);
            Assert.AreEqual(expectedRoute, _httpClient.Requests[0].Uri);

            Assert.IsNull(_httpClient.Requests[0].Content);
            Assert.IsNotNull(ports);
            Assert.HasCount(1, ports);
            Assert.AreEqual(port.Id, ports[0].Id);
            Assert.AreEqual(port.Code, ports[0].Code);
            Assert.AreEqual(port.Name, ports[0].Name);
        }
    }
}
