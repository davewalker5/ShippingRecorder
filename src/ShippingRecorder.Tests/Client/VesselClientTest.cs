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
using System.IO;
using System.Linq;

namespace ShippingRecorder.Tests.Client
{
    [TestClass]
    public class VesselClientTest
    {
        private readonly string ApiToken = "An API Token";
        private readonly MockShippingRecorderHttpClient _httpClient = new();
        private IVesselClient _client;
        private string _filePath;

        private readonly ShippingRecorderApplicationSettings _settings = new()
        {
            ApiUrl = "http://server/",
            ApiRoutes = [
                new() { Name = "Vessel", Route = "/vessels" },
                new() { Name = "ImportVessel", Route = "/import/vessels" },
                new() { Name = "ExportVessel", Route = "/export/vessels" }
            ]
        };

        [TestInitialize]
        public void Initialise()
        {
            var provider = new Mock<IAuthenticationTokenProvider>();
            provider.Setup(x => x.GetToken()).Returns(ApiToken);
            var logger = new Mock<ILogger<VesselClient>>();
            var cache = new Mock<ICacheWrapper>();
            _client = new VesselClient(_httpClient, _settings, provider.Object, cache.Object, logger.Object);
        }

        [TestCleanup]
        public void CleanUp()
        {
            if (!string.IsNullOrEmpty(_filePath) && File.Exists(_filePath))
            {
                File.Delete(_filePath);
            }
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
            Assert.AreEqual(_settings.ApiRoutes.First(x => x.Name == "Vessel").Route, _httpClient.Requests[0].Uri);

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
            Assert.AreEqual(_settings.ApiRoutes.First(x => x.Name == "Vessel").Route, _httpClient.Requests[0].Uri);

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
            Assert.AreEqual($"{_settings.ApiRoutes.First(x => x.Name == "Vessel").Route}/{id}", _httpClient.Requests[0].Uri);

            Assert.IsNull(_httpClient.Requests[0].Content);
        }

        [TestMethod]
        public async Task GetByIdTest()
        {
            var vessel = DataGenerator.CreateVessel();
            var json = JsonSerializer.Serialize(vessel);
            _httpClient.AddResponse(json);

            var retrieved = await _client.GetAsync(vessel.Id);
            var expectedRoute = $"{_settings.ApiRoutes.First(x => x.Name == "Vessel").Route}/{vessel.Id}";

            Assert.AreEqual($"Bearer {ApiToken}", _httpClient.DefaultRequestHeaders.Authorization.ToString());
            Assert.AreEqual($"{_settings.ApiUrl}", _httpClient.BaseAddress.ToString());
            Assert.AreEqual(HttpMethod.Get, _httpClient.Requests[0].Method);
            Assert.AreEqual(expectedRoute, _httpClient.Requests[0].Uri);

            Assert.IsNull(_httpClient.Requests[0].Content);
            Assert.IsNotNull(retrieved);
            Assert.AreEqual(vessel.Id, retrieved.Id);
            Assert.AreEqual(vessel.IMO, retrieved.IMO);
            Assert.AreEqual(vessel.Built, retrieved.Built);
            Assert.AreEqual(vessel.Draught, retrieved.Draught);
            Assert.AreEqual(vessel.Length, retrieved.Length);
            Assert.AreEqual(vessel.Beam, retrieved.Beam);
        }

        [TestMethod]
        public async Task GetByIMOTest()
        {
            var vessel = DataGenerator.CreateVessel();
            var json = JsonSerializer.Serialize(vessel);
            _httpClient.AddResponse(json);

            var retrieved = await _client.GetAsync(vessel.IMO);
            var expectedRoute = $"{_settings.ApiRoutes.First(x => x.Name == "Vessel").Route}/imo/{vessel.IMO}";

            Assert.AreEqual($"Bearer {ApiToken}", _httpClient.DefaultRequestHeaders.Authorization.ToString());
            Assert.AreEqual($"{_settings.ApiUrl}", _httpClient.BaseAddress.ToString());
            Assert.AreEqual(HttpMethod.Get, _httpClient.Requests[0].Method);
            Assert.AreEqual(expectedRoute, _httpClient.Requests[0].Uri);

            Assert.IsNull(_httpClient.Requests[0].Content);
            Assert.IsNotNull(retrieved);
            Assert.AreEqual(vessel.Id, retrieved.Id);
            Assert.AreEqual(vessel.IMO, retrieved.IMO);
            Assert.AreEqual(vessel.Built, retrieved.Built);
            Assert.AreEqual(vessel.Draught, retrieved.Draught);
            Assert.AreEqual(vessel.Length, retrieved.Length);
            Assert.AreEqual(vessel.Beam, retrieved.Beam);
        }

        [TestMethod]
        public async Task ListTest()
        {
            var vessel = DataGenerator.CreateVessel();
            var json = JsonSerializer.Serialize<List<Vessel>>([vessel]);
            _httpClient.AddResponse(json);

            var vessels = await _client.ListAsync(1, int.MaxValue);
            var expectedRoute = $"{_settings.ApiRoutes.First(x => x.Name == "Vessel").Route}/1/{int.MaxValue}";

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

        [TestMethod]
        public async Task ImportFromFileTest()
        {
            var vessel = DataGenerator.CreateVessel();
            var record = $@"""{vessel.IMO}"",""{vessel.Built}"",""{vessel.Draught}"",""{vessel.Length}"",""{vessel.Beam}"",""{vessel.ActiveRegistrationHistory.Tonnage}"",""{vessel.ActiveRegistrationHistory.Passengers}"",""{vessel.ActiveRegistrationHistory.Crew}"",""{vessel.ActiveRegistrationHistory.Decks}"",""{vessel.ActiveRegistrationHistory.Cabins}"",""{vessel.ActiveRegistrationHistory.Name}"",""{vessel.ActiveRegistrationHistory.Callsign}"",""{vessel.ActiveRegistrationHistory.MMSI}"",""{vessel.ActiveRegistrationHistory.VesselType.Name}"",""{vessel.ActiveRegistrationHistory.Flag}"",""{vessel.ActiveRegistrationHistory.Operator.Name}""";
            _filePath = Path.ChangeExtension(Path.GetTempFileName(), "csv");
            File.WriteAllLines(_filePath, ["", record]);
            _httpClient.AddResponse("");

            var content = File.ReadAllText(_filePath);
            var json = JsonSerializer.Serialize(new { Content = content });
            var expectedRoute = _settings.ApiRoutes.First(x => x.Name == "ImportVessel").Route;

            await _client.ImportFromFileAsync(_filePath);

            Assert.AreEqual($"Bearer {ApiToken}", _httpClient.DefaultRequestHeaders.Authorization.ToString());
            Assert.AreEqual($"{_settings.ApiUrl}", _httpClient.BaseAddress.ToString());
            Assert.AreEqual(HttpMethod.Post, _httpClient.Requests[0].Method);
            Assert.AreEqual(expectedRoute, _httpClient.Requests[0].Uri);
            Assert.AreEqual(json, await _httpClient.Requests[0].Content.ReadAsStringAsync());
        }

        [TestMethod]
        public async Task ExportTest()
        {
            _filePath = Path.ChangeExtension(Path.GetTempFileName(), "csv");
            _httpClient.AddResponse("");

            var json = JsonSerializer.Serialize(new { FileName = _filePath });
            var expectedRoute = _settings.ApiRoutes.First(x => x.Name == "ExportVessel").Route;

            await _client.ExportAsync(_filePath);

            Assert.AreEqual($"Bearer {ApiToken}", _httpClient.DefaultRequestHeaders.Authorization.ToString());
            Assert.AreEqual($"{_settings.ApiUrl}", _httpClient.BaseAddress.ToString());
            Assert.AreEqual(HttpMethod.Post, _httpClient.Requests[0].Method);
            Assert.AreEqual(expectedRoute, _httpClient.Requests[0].Uri);
            Assert.AreEqual(json, await _httpClient.Requests[0].Content.ReadAsStringAsync());
        }
    }
}
