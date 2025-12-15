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
    public class RegistrationHistoryClientTest
    {
        private readonly string ApiToken = "An API Token";
        private readonly MockShippingRecorderHttpClient _httpClient = new();
        private IRegistrationHistoryClient _client;

        private readonly ShippingRecorderApplicationSettings _settings = new()
        {
            ApiUrl = "http://server/",
            ApiRoutes = [
                new() { Name = "RegistrationHistory", Route = "/registrationhistory" }
            ]
        };

        [TestInitialize]
        public void Initialise()
        {
            var provider = new Mock<IAuthenticationTokenProvider>();
            provider.Setup(x => x.GetToken()).Returns(ApiToken);
            var logger = new Mock<ILogger<RegistrationHistoryClient>>();
            _client = new RegistrationHistoryClient(_httpClient, _settings, provider.Object, logger.Object);
        }

        [TestMethod]
        public async Task AddTest()
        {
            var history = DataGenerator.CreateRegistrationHistory();
            var json = JsonSerializer.Serialize(new
            {
                history.VesselId,
                history.VesselTypeId,
                history.FlagId,
                history.OperatorId,
                history.Date,
                history.Name,
                history.Callsign,
                history.MMSI,
                history.Tonnage,
                history.Passengers,
                history.Crew,
                history.Decks,
                history.Cabins
            });
            _httpClient.AddResponse(json);

            var added = await _client.AddAsync(
                history.VesselId,
                history.VesselTypeId,
                history.FlagId,
                history.OperatorId,
                history.Date,
                history.Name,
                history.Callsign,
                history.MMSI,
                history.Tonnage,
                history.Passengers,
                history.Crew,
                history.Decks,
                history.Cabins);

            Assert.AreEqual($"Bearer {ApiToken}", _httpClient.DefaultRequestHeaders.Authorization.ToString());
            Assert.AreEqual($"{_settings.ApiUrl}", _httpClient.BaseAddress.ToString());
            Assert.AreEqual(HttpMethod.Post, _httpClient.Requests[0].Method);
            Assert.AreEqual(_settings.ApiRoutes[0].Route, _httpClient.Requests[0].Uri);

            Assert.StartsWith((await _httpClient.Requests[0].Content.ReadAsStringAsync())[..^1], json);
            Assert.IsNotNull(added);
            Assert.AreEqual(history.VesselId, added.VesselId);
            Assert.AreEqual(history.VesselTypeId, added.VesselTypeId);
            Assert.AreEqual(history.FlagId, added.FlagId);
            Assert.AreEqual(history.OperatorId, added.OperatorId);
            Assert.AreEqual(history.Date, added.Date);
            Assert.AreEqual(history.Name, added.Name);
            Assert.AreEqual(history.Callsign, added.Callsign);
            Assert.AreEqual(history.MMSI, added.MMSI);
            Assert.AreEqual(history.Tonnage, added.Tonnage);
            Assert.AreEqual(history.Passengers, added.Passengers);
            Assert.AreEqual(history.Crew, added.Crew);
            Assert.AreEqual(history.Decks, added.Decks);
            Assert.AreEqual(history.Cabins, added.Cabins);
        }

        [TestMethod]
        public async Task DeleteTest()
        {
            var id = DataGenerator.RandomId();
            await _client.Deactivate(id);

            Assert.AreEqual($"Bearer {ApiToken}", _httpClient.DefaultRequestHeaders.Authorization.ToString());
            Assert.AreEqual($"{_settings.ApiUrl}", _httpClient.BaseAddress.ToString());
            Assert.AreEqual(HttpMethod.Delete, _httpClient.Requests[0].Method);
            Assert.AreEqual($"{_settings.ApiRoutes[0].Route}/{id}", _httpClient.Requests[0].Uri);

            Assert.IsNull(_httpClient.Requests[0].Content);
        }

        [TestMethod]
        public async Task ListTest()
        {
            var history = DataGenerator.CreateRegistrationHistory();
            var json = JsonSerializer.Serialize<List<RegistrationHistory>>([history]);
            _httpClient.AddResponse(json);

            var histories = await _client.ListAsync(history.VesselId, 1, int.MaxValue);
            var expectedRoute = $"{_settings.ApiRoutes[0].Route}/{history.VesselId}/1/{int.MaxValue}";

            Assert.AreEqual($"Bearer {ApiToken}", _httpClient.DefaultRequestHeaders.Authorization.ToString());
            Assert.AreEqual($"{_settings.ApiUrl}", _httpClient.BaseAddress.ToString());
            Assert.AreEqual(HttpMethod.Get, _httpClient.Requests[0].Method);
            Assert.AreEqual(expectedRoute, _httpClient.Requests[0].Uri);

            Assert.IsNull(_httpClient.Requests[0].Content);
            Assert.IsNotNull(histories);
            Assert.HasCount(1, histories);
            Assert.AreEqual(history.Id, histories[0].Id);
            Assert.AreEqual(history.VesselId, histories[0].VesselId);
            Assert.AreEqual(history.VesselTypeId, histories[0].VesselTypeId);
            Assert.AreEqual(history.FlagId, histories[0].FlagId);
            Assert.AreEqual(history.OperatorId, histories[0].OperatorId);
            Assert.AreEqual(history.Date, histories[0].Date);
            Assert.AreEqual(history.Name, histories[0].Name);
            Assert.AreEqual(history.Callsign, histories[0].Callsign);
            Assert.AreEqual(history.MMSI, histories[0].MMSI);
            Assert.AreEqual(history.Tonnage, histories[0].Tonnage);
            Assert.AreEqual(history.Passengers, histories[0].Passengers);
            Assert.AreEqual(history.Crew, histories[0].Crew);
            Assert.AreEqual(history.Decks, histories[0].Decks);
            Assert.AreEqual(history.Cabins, histories[0].Cabins);
        }
    }
}
