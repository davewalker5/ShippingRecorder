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
using System.Linq;
using System.IO;
using ShippingRecorder.DataExchange.Entities;

namespace ShippingRecorder.Tests.Client
{
    [TestClass]
    public class VoyageClientTest
    {
        private readonly string ApiToken = "An API Token";
        private readonly MockShippingRecorderHttpClient _httpClient = new();
        private IVoyageClient _client;
        private string _filePath;

        private readonly ShippingRecorderApplicationSettings _settings = new()
        {
            ApiUrl = "http://server/",
            ApiRoutes = [
                new() { Name = "Voyage", Route = "/voyages" },
                new() { Name = "ImportVoyage", Route = "/import/voyages" },
                new() { Name = "ExportVoyage", Route = "/export/voyages" }
            ]
        };

        [TestInitialize]
        public void Initialise()
        {
            var provider = new Mock<IAuthenticationTokenProvider>();
            provider.Setup(x => x.GetToken()).Returns(ApiToken);
            var logger = new Mock<ILogger<VoyageClient>>();
            var cache = new Mock<ICacheWrapper>();
            _client = new VoyageClient(_httpClient, _settings, provider.Object, cache.Object, logger.Object);
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
            var voyage = DataGenerator.CreateVoyage();
            var json = JsonSerializer.Serialize(new { voyage.OperatorId, voyage.VesselId, voyage.Number });
            _httpClient.AddResponse(json);

            var added = await _client.AddAsync(voyage.OperatorId, voyage.VesselId, voyage.Number);

            Assert.AreEqual($"Bearer {ApiToken}", _httpClient.DefaultRequestHeaders.Authorization.ToString());
            Assert.AreEqual($"{_settings.ApiUrl}", _httpClient.BaseAddress.ToString());
            Assert.AreEqual(HttpMethod.Post, _httpClient.Requests[0].Method);
            Assert.AreEqual(_settings.ApiRoutes.First(x => x.Name == "Voyage").Route, _httpClient.Requests[0].Uri);

            Assert.AreEqual(json, await _httpClient.Requests[0].Content.ReadAsStringAsync());
            Assert.IsNotNull(added);
            Assert.AreEqual(voyage.OperatorId, added.OperatorId);
            Assert.AreEqual(voyage.Number, added.Number);
        }

        [TestMethod]
        public async Task UpdateTest()
        {
            var voyage = DataGenerator.CreateVoyage();
            var json = JsonSerializer.Serialize(voyage);
            _httpClient.AddResponse(json);

            var updated = await _client.UpdateAsync(voyage.Id, voyage.OperatorId, voyage.VesselId, voyage.Number);

            Assert.AreEqual($"Bearer {ApiToken}", _httpClient.DefaultRequestHeaders.Authorization.ToString());
            Assert.AreEqual($"{_settings.ApiUrl}", _httpClient.BaseAddress.ToString());
            Assert.AreEqual(HttpMethod.Put, _httpClient.Requests[0].Method);
            Assert.AreEqual(_settings.ApiRoutes.First(x => x.Name == "Voyage").Route, _httpClient.Requests[0].Uri);

            Assert.StartsWith((await _httpClient.Requests[0].Content.ReadAsStringAsync())[..^1], json);
            Assert.IsNotNull(updated);
            Assert.AreEqual(voyage.Id, updated.Id);
            Assert.AreEqual(voyage.OperatorId, updated.OperatorId);
            Assert.AreEqual(voyage.Number, updated.Number);
        }

        [TestMethod]
        public async Task DeleteTest()
        {
            var id = DataGenerator.RandomId();
            await _client.DeleteAsync(id);

            Assert.AreEqual($"Bearer {ApiToken}", _httpClient.DefaultRequestHeaders.Authorization.ToString());
            Assert.AreEqual($"{_settings.ApiUrl}", _httpClient.BaseAddress.ToString());
            Assert.AreEqual(HttpMethod.Delete, _httpClient.Requests[0].Method);
            Assert.AreEqual($"{_settings.ApiRoutes.First(x => x.Name == "Voyage").Route}/{id}", _httpClient.Requests[0].Uri);

            Assert.IsNull(_httpClient.Requests[0].Content);
        }

        [TestMethod]
        public async Task GetTest()
        {
            var voyage = DataGenerator.CreateVoyage();
            var json = JsonSerializer.Serialize(voyage);
            _httpClient.AddResponse(json);

            var retrieved = await _client.GetAsync(voyage.Id);
            var expectedRoute = $"{_settings.ApiRoutes.First(x => x.Name == "Voyage").Route}/{voyage.Id}";

            Assert.AreEqual($"Bearer {ApiToken}", _httpClient.DefaultRequestHeaders.Authorization.ToString());
            Assert.AreEqual($"{_settings.ApiUrl}", _httpClient.BaseAddress.ToString());
            Assert.AreEqual(HttpMethod.Get, _httpClient.Requests[0].Method);
            Assert.AreEqual(expectedRoute, _httpClient.Requests[0].Uri);

            Assert.IsNull(_httpClient.Requests[0].Content);
            Assert.IsNotNull(retrieved);
            Assert.AreEqual(voyage.Id, retrieved.Id);
            Assert.AreEqual(voyage.OperatorId, retrieved.OperatorId);
            Assert.AreEqual(voyage.VesselId, retrieved.VesselId);
            Assert.AreEqual(voyage.Number, retrieved.Number);
        }

        [TestMethod]
        public async Task ListTest()
        {
            var voyage = DataGenerator.CreateVoyage();
            var json = JsonSerializer.Serialize<List<Voyage>>([voyage]);
            _httpClient.AddResponse(json);

            var voyages = await _client.ListAsync(voyage.OperatorId, 1, int.MaxValue);
            var expectedRoute = $"{_settings.ApiRoutes.First(x => x.Name == "Voyage").Route}/{voyage.OperatorId}/1/{int.MaxValue}";

            Assert.AreEqual($"Bearer {ApiToken}", _httpClient.DefaultRequestHeaders.Authorization.ToString());
            Assert.AreEqual($"{_settings.ApiUrl}", _httpClient.BaseAddress.ToString());
            Assert.AreEqual(HttpMethod.Get, _httpClient.Requests[0].Method);
            Assert.AreEqual(expectedRoute, _httpClient.Requests[0].Uri);

            Assert.IsNull(_httpClient.Requests[0].Content);
            Assert.IsNotNull(voyages);
            Assert.HasCount(1, voyages);
            Assert.AreEqual(voyage.Id, voyages[0].Id);
            Assert.AreEqual(voyage.OperatorId, voyages[0].OperatorId);
            Assert.AreEqual(voyage.Number, voyages[0].Number);
        }

        [TestMethod]
        public async Task ImportFromFileTest()
        {
            var voyage = DataGenerator.CreateVoyage();
            var record = $@"""{voyage.Vessel.IMO}"",""{voyage.Operator.Name}"",""{voyage.Number}"",""{voyage.Events.First().EventType}"",""{voyage.Events.First().Port.Code}"",""{voyage.Events.First().Date.ToString(ExportableVoyage.DateFormat)}""";
            _filePath = Path.ChangeExtension(Path.GetTempFileName(), "csv");
            File.WriteAllLines(_filePath, ["", record]);
            _httpClient.AddResponse("");

            var content = File.ReadAllText(_filePath);
            var json = JsonSerializer.Serialize(new { Content = content });
            var expectedRoute = _settings.ApiRoutes.First(x => x.Name == "ImportVoyage").Route;

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
            var expectedRoute = _settings.ApiRoutes.First(x => x.Name == "ExportVoyage").Route;

            await _client.ExportAsync(_filePath);

            Assert.AreEqual($"Bearer {ApiToken}", _httpClient.DefaultRequestHeaders.Authorization.ToString());
            Assert.AreEqual($"{_settings.ApiUrl}", _httpClient.BaseAddress.ToString());
            Assert.AreEqual(HttpMethod.Post, _httpClient.Requests[0].Method);
            Assert.AreEqual(expectedRoute, _httpClient.Requests[0].Uri);
            Assert.AreEqual(json, await _httpClient.Requests[0].Content.ReadAsStringAsync());
        }
    }
}
