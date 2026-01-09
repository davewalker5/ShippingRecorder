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
using System.Web;
using System;

namespace ShippingRecorder.Tests.Client
{
    [TestClass]
    public class SightingClientTest
    {
        private readonly string ApiToken = "An API Token";
        private readonly MockShippingRecorderHttpClient _httpClient = new();
        private ISightingClient _client;

        private readonly ShippingRecorderApplicationSettings _settings = new()
        {
            ApiUrl = "http://server/",
            ApiRoutes = [
                new() { Name = "Sighting", Route = "/sightings" }
            ]
        };

        [TestInitialize]
        public void Initialise()
        {
            var provider = new Mock<IAuthenticationTokenProvider>();
            provider.Setup(x => x.GetToken()).Returns(ApiToken);
            var logger = new Mock<ILogger<SightingClient>>();
            var cache = new Mock<ICacheWrapper>();
            _client = new SightingClient(_httpClient, _settings, provider.Object, cache.Object, logger.Object);
        }

        [TestMethod]
        public async Task AddTest()
        {
            var sighting = DataGenerator.CreateSighting();
            var json = JsonSerializer.Serialize(new
            {
                sighting.LocationId,
                sighting.VoyageId,
                sighting.VesselId,
                sighting.Date,
                sighting.IsMyVoyage
            });
            _httpClient.AddResponse(json);

            var added = await _client.AddAsync(sighting.LocationId, sighting.VoyageId, sighting.VesselId, sighting.Date, sighting.IsMyVoyage);

            Assert.AreEqual($"Bearer {ApiToken}", _httpClient.DefaultRequestHeaders.Authorization.ToString());
            Assert.AreEqual($"{_settings.ApiUrl}", _httpClient.BaseAddress.ToString());
            Assert.AreEqual(HttpMethod.Post, _httpClient.Requests[0].Method);
            Assert.AreEqual(_settings.ApiRoutes[0].Route, _httpClient.Requests[0].Uri);

            Assert.AreEqual(json, await _httpClient.Requests[0].Content.ReadAsStringAsync());
            Assert.IsNotNull(added);
            Assert.AreEqual(sighting.LocationId, added.LocationId);
            Assert.AreEqual(sighting.VoyageId, added.VoyageId);
            Assert.AreEqual(sighting.VesselId, added.VesselId);
            Assert.AreEqual(sighting.Date, added.Date);
            Assert.AreEqual(sighting.IsMyVoyage, added.IsMyVoyage);
        }

        [TestMethod]
        public async Task UpdateTest()
        {
            var sighting = DataGenerator.CreateSighting();
            var json = JsonSerializer.Serialize(sighting);
            _httpClient.AddResponse(json);

            var updated = await _client.UpdateAsync(sighting.Id, sighting.LocationId, sighting.VoyageId, sighting.VesselId, sighting.Date, sighting.IsMyVoyage);

            Assert.AreEqual($"Bearer {ApiToken}", _httpClient.DefaultRequestHeaders.Authorization.ToString());
            Assert.AreEqual($"{_settings.ApiUrl}", _httpClient.BaseAddress.ToString());
            Assert.AreEqual(HttpMethod.Put, _httpClient.Requests[0].Method);
            Assert.AreEqual(_settings.ApiRoutes[0].Route, _httpClient.Requests[0].Uri);

            Assert.StartsWith((await _httpClient.Requests[0].Content.ReadAsStringAsync())[..^1], json);
            Assert.IsNotNull(updated);
            Assert.AreEqual(sighting.LocationId, updated.LocationId);
            Assert.AreEqual(sighting.VoyageId, updated.VoyageId);
            Assert.AreEqual(sighting.VesselId, updated.VesselId);
            Assert.AreEqual(sighting.Date, updated.Date);
            Assert.AreEqual(sighting.IsMyVoyage, updated.IsMyVoyage);
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
        public async Task GetTest()
        {
            var sighting = DataGenerator.CreateSighting();
            var json = JsonSerializer.Serialize(sighting);
            _httpClient.AddResponse(json);

            var retrieved = await _client.GetAsync(sighting.Id);
            var expectedRoute = $"{_settings.ApiRoutes[0].Route}/{sighting.Id}";

            Assert.AreEqual($"Bearer {ApiToken}", _httpClient.DefaultRequestHeaders.Authorization.ToString());
            Assert.AreEqual($"{_settings.ApiUrl}", _httpClient.BaseAddress.ToString());
            Assert.AreEqual(HttpMethod.Get, _httpClient.Requests[0].Method);
            Assert.AreEqual(expectedRoute, _httpClient.Requests[0].Uri);

            Assert.IsNull(_httpClient.Requests[0].Content);
            Assert.IsNotNull(retrieved);
            Assert.AreEqual(sighting.Id, retrieved.Id);
            Assert.AreEqual(sighting.LocationId, retrieved.LocationId);
            Assert.AreEqual(sighting.VoyageId, retrieved.VoyageId);
            Assert.AreEqual(sighting.VesselId, retrieved.VesselId);
            Assert.AreEqual(sighting.Date, retrieved.Date);
            Assert.AreEqual(sighting.IsMyVoyage, retrieved.IsMyVoyage);
        }

        [TestMethod]
        public async Task GetMostRecentTest()
        {
            var imo = DataGenerator.RandomInt(0, 9999999).ToString();
            var sighting = DataGenerator.CreateSighting();
            var json = JsonSerializer.Serialize<List<Sighting>>([sighting]);
            _httpClient.AddResponse(json);

            var retrieved = await _client.GetMostRecentVesselSightingAsync(imo);
            var expectedRoute = $"{_settings.ApiRoutes[0].Route}/vessel/{imo}";

            Assert.AreEqual($"Bearer {ApiToken}", _httpClient.DefaultRequestHeaders.Authorization.ToString());
            Assert.AreEqual($"{_settings.ApiUrl}", _httpClient.BaseAddress.ToString());
            Assert.AreEqual(HttpMethod.Get, _httpClient.Requests[0].Method);
            Assert.StartsWith(expectedRoute, _httpClient.Requests[0].Uri);

            Assert.IsNull(_httpClient.Requests[0].Content);
            Assert.IsNotNull(retrieved);
            Assert.AreEqual(sighting.Id, retrieved.Id);
            Assert.AreEqual(sighting.LocationId, retrieved.LocationId);
            Assert.AreEqual(sighting.VoyageId, retrieved.VoyageId);
            Assert.AreEqual(sighting.VesselId, retrieved.VesselId);
            Assert.AreEqual(sighting.Date, retrieved.Date);
            Assert.AreEqual(sighting.IsMyVoyage, retrieved.IsMyVoyage);
        }

        [TestMethod]
        public async Task ListTest()
        {
            var sighting = DataGenerator.CreateSighting();
            var json = JsonSerializer.Serialize<List<Sighting>>([sighting]);
            _httpClient.AddResponse(json);

            var sightings = await _client.ListAsync(1, int.MaxValue);
            var expectedRoute = $"{_settings.ApiRoutes[0].Route}/1/{int.MaxValue}";

            Assert.AreEqual($"Bearer {ApiToken}", _httpClient.DefaultRequestHeaders.Authorization.ToString());
            Assert.AreEqual($"{_settings.ApiUrl}", _httpClient.BaseAddress.ToString());
            Assert.AreEqual(HttpMethod.Get, _httpClient.Requests[0].Method);
            Assert.AreEqual(expectedRoute, _httpClient.Requests[0].Uri);

            Assert.IsNull(_httpClient.Requests[0].Content);
            Assert.IsNotNull(sightings);
            Assert.HasCount(1, sightings);
            Assert.AreEqual(sighting.Id, sightings[0].Id);
            Assert.AreEqual(sighting.LocationId, sightings[0].LocationId);
            Assert.AreEqual(sighting.VoyageId, sightings[0].VoyageId);
            Assert.AreEqual(sighting.VesselId, sightings[0].VesselId);
            Assert.AreEqual(sighting.Date, sightings[0].Date);
            Assert.AreEqual(sighting.IsMyVoyage, sightings[0].IsMyVoyage);
        }

        [TestMethod]
        public async Task ListByVesselTest()
        {
            var sighting = DataGenerator.CreateSighting();
            var json = JsonSerializer.Serialize<List<Sighting>>([sighting]);
            _httpClient.AddResponse(json);

            var sightings = await _client.ListSightingsByVesselAsync(sighting.VesselId, 1, int.MaxValue);
            var expectedRoute = $"{_settings.ApiRoutes[0].Route}/vessel/{sighting.VesselId}/1/{int.MaxValue}";

            Assert.AreEqual($"Bearer {ApiToken}", _httpClient.DefaultRequestHeaders.Authorization.ToString());
            Assert.AreEqual($"{_settings.ApiUrl}", _httpClient.BaseAddress.ToString());
            Assert.AreEqual(HttpMethod.Get, _httpClient.Requests[0].Method);
            Assert.AreEqual(expectedRoute, _httpClient.Requests[0].Uri);

            Assert.IsNull(_httpClient.Requests[0].Content);
            Assert.IsNotNull(sightings);
            Assert.HasCount(1, sightings);
            Assert.AreEqual(sighting.Id, sightings[0].Id);
            Assert.AreEqual(sighting.LocationId, sightings[0].LocationId);
            Assert.AreEqual(sighting.VoyageId, sightings[0].VoyageId);
            Assert.AreEqual(sighting.VesselId, sightings[0].VesselId);
            Assert.AreEqual(sighting.Date, sightings[0].Date);
            Assert.AreEqual(sighting.IsMyVoyage, sightings[0].IsMyVoyage);
        }

        [TestMethod]
        public async Task ListByLocationTest()
        {
            var sighting = DataGenerator.CreateSighting();
            var json = JsonSerializer.Serialize<List<Sighting>>([sighting]);
            _httpClient.AddResponse(json);

            var sightings = await _client.ListSightingsByLocationAsync(sighting.LocationId, 1, int.MaxValue);
            var expectedRoute = $"{_settings.ApiRoutes[0].Route}/location/{sighting.LocationId}";

            Assert.AreEqual($"Bearer {ApiToken}", _httpClient.DefaultRequestHeaders.Authorization.ToString());
            Assert.AreEqual($"{_settings.ApiUrl}", _httpClient.BaseAddress.ToString());
            Assert.AreEqual(HttpMethod.Get, _httpClient.Requests[0].Method);
            Assert.StartsWith(expectedRoute, _httpClient.Requests[0].Uri);

            Assert.IsNull(_httpClient.Requests[0].Content);
            Assert.IsNotNull(sightings);
            Assert.HasCount(1, sightings);
            Assert.AreEqual(sighting.Id, sightings[0].Id);
            Assert.AreEqual(sighting.LocationId, sightings[0].LocationId);
            Assert.AreEqual(sighting.VoyageId, sightings[0].VoyageId);
            Assert.AreEqual(sighting.VesselId, sightings[0].VesselId);
            Assert.AreEqual(sighting.Date, sightings[0].Date);
            Assert.AreEqual(sighting.IsMyVoyage, sightings[0].IsMyVoyage);
        }

        [TestMethod]
        public async Task ListByDateTest()
        {
            var sighting = DataGenerator.CreateSighting();
            var json = JsonSerializer.Serialize<List<Sighting>>([sighting]);
            _httpClient.AddResponse(json);

            var from = sighting.Date.AddDays(-1);
            var to = sighting.Date.AddDays(1);

            var sightings = await _client.ListSightingsByDateAsync(from, to, 1, int.MaxValue);
            var expectedRoute = $"{_settings.ApiRoutes[0].Route}/date/";

            Assert.AreEqual($"Bearer {ApiToken}", _httpClient.DefaultRequestHeaders.Authorization.ToString());
            Assert.AreEqual($"{_settings.ApiUrl}", _httpClient.BaseAddress.ToString());
            Assert.AreEqual(HttpMethod.Get, _httpClient.Requests[0].Method);
            Assert.StartsWith(expectedRoute, _httpClient.Requests[0].Uri);

            Assert.IsNull(_httpClient.Requests[0].Content);
            Assert.IsNotNull(sightings);
            Assert.HasCount(1, sightings);
            Assert.AreEqual(sighting.Id, sightings[0].Id);
            Assert.AreEqual(sighting.LocationId, sightings[0].LocationId);
            Assert.AreEqual(sighting.VoyageId, sightings[0].VoyageId);
            Assert.AreEqual(sighting.VesselId, sightings[0].VesselId);
            Assert.AreEqual(sighting.Date, sightings[0].Date);
            Assert.AreEqual(sighting.IsMyVoyage, sightings[0].IsMyVoyage);
        }

        [TestMethod]
        public async Task ListMyVoyagesTest()
        {
            var sighting = DataGenerator.CreateSighting();
            sighting.IsMyVoyage = true;
            var json = JsonSerializer.Serialize<List<Sighting>>([sighting]);
            _httpClient.AddResponse(json);

            var sightings = await _client.ListMyVoyagesAsync(1, int.MaxValue);
            var expectedRoute = $"{_settings.ApiRoutes[0].Route}/mine/1/{int.MaxValue}";

            Assert.AreEqual($"Bearer {ApiToken}", _httpClient.DefaultRequestHeaders.Authorization.ToString());
            Assert.AreEqual($"{_settings.ApiUrl}", _httpClient.BaseAddress.ToString());
            Assert.AreEqual(HttpMethod.Get, _httpClient.Requests[0].Method);
            Assert.AreEqual(expectedRoute, _httpClient.Requests[0].Uri);

            Assert.IsNull(_httpClient.Requests[0].Content);
            Assert.IsNotNull(sightings);
            Assert.HasCount(1, sightings);
            Assert.AreEqual(sighting.Id, sightings[0].Id);
            Assert.AreEqual(sighting.LocationId, sightings[0].LocationId);
            Assert.AreEqual(sighting.VoyageId, sightings[0].VoyageId);
            Assert.AreEqual(sighting.VesselId, sightings[0].VesselId);
            Assert.AreEqual(sighting.Date, sightings[0].Date);
            Assert.AreEqual(sighting.IsMyVoyage, sightings[0].IsMyVoyage);
        }
    }
}
