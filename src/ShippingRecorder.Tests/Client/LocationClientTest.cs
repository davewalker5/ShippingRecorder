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

namespace ShippingRecorder.Tests.Client
{
    [TestClass]
    public class LocationClientTest
    {
        private readonly string ApiToken = "An API Token";
        private readonly MockShippingRecorderHttpClient _httpClient = new();
        private ILocationClient _client;
        private string _filePath;

        private readonly ShippingRecorderApplicationSettings _settings = new()
        {
            ApiUrl = "http://server/",
            ApiRoutes = [
                new() { Name = "Location", Route = "/locations" },
                new() { Name = "ImportLocation", Route = "/import/locations" },
                new() { Name = "ExportLocation", Route = "/export/locations" }
            ]
        };

        [TestInitialize]
        public void Initialise()
        {
            var provider = new Mock<IAuthenticationTokenProvider>();
            provider.Setup(x => x.GetToken()).Returns(ApiToken);
            var logger = new Mock<ILogger<LocationClient>>();
            var cache = new Mock<ICacheWrapper>();
            _client = new LocationClient(_httpClient, _settings, provider.Object, cache.Object, logger.Object);
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
            var location = DataGenerator.CreateLocation();
            var json = JsonSerializer.Serialize(new { location.Name });
            _httpClient.AddResponse(json);

            var added = await _client.AddAsync(location.Name);

            Assert.AreEqual($"Bearer {ApiToken}", _httpClient.DefaultRequestHeaders.Authorization.ToString());
            Assert.AreEqual($"{_settings.ApiUrl}", _httpClient.BaseAddress.ToString());
            Assert.AreEqual(HttpMethod.Post, _httpClient.Requests[0].Method);
            Assert.AreEqual(_settings.ApiRoutes.First(x => x.Name == "Location").Route, _httpClient.Requests[0].Uri);

            Assert.AreEqual(json, await _httpClient.Requests[0].Content.ReadAsStringAsync());
            Assert.IsNotNull(added);
            Assert.AreEqual(location.Name, added.Name);
        }

        [TestMethod]
        public async Task UpdateTest()
        {
            var location = DataGenerator.CreateLocation();
            var json = JsonSerializer.Serialize(location);
            _httpClient.AddResponse(json);

            var updated = await _client.UpdateAsync(location.Id, location.Name);

            Assert.AreEqual($"Bearer {ApiToken}", _httpClient.DefaultRequestHeaders.Authorization.ToString());
            Assert.AreEqual($"{_settings.ApiUrl}", _httpClient.BaseAddress.ToString());
            Assert.AreEqual(HttpMethod.Put, _httpClient.Requests[0].Method);
            Assert.AreEqual(_settings.ApiRoutes.First(x => x.Name == "Location").Route, _httpClient.Requests[0].Uri);

            Assert.AreEqual(json, await _httpClient.Requests[0].Content.ReadAsStringAsync());
            Assert.IsNotNull(updated);
            Assert.AreEqual(location.Id, updated.Id);
            Assert.AreEqual(location.Name, updated.Name);
        }

        [TestMethod]
        public async Task DeleteTest()
        {
            var id = DataGenerator.RandomId();
            await _client.DeleteAsync(id);

            Assert.AreEqual($"Bearer {ApiToken}", _httpClient.DefaultRequestHeaders.Authorization.ToString());
            Assert.AreEqual($"{_settings.ApiUrl}", _httpClient.BaseAddress.ToString());
            Assert.AreEqual(HttpMethod.Delete, _httpClient.Requests[0].Method);
            Assert.AreEqual($"{_settings.ApiRoutes.First(x => x.Name == "Location").Route}/{id}", _httpClient.Requests[0].Uri);

            Assert.IsNull(_httpClient.Requests[0].Content);
        }

        [TestMethod]
        public async Task GetTest()
        {
            var location = DataGenerator.CreateLocation();
            var json = JsonSerializer.Serialize(location);
            _httpClient.AddResponse(json);

            var retrieved = await _client.GetAsync(location.Id);
            var expectedRoute = $"{_settings.ApiRoutes.First(x => x.Name == "Location").Route}/{location.Id}";

            Assert.AreEqual($"Bearer {ApiToken}", _httpClient.DefaultRequestHeaders.Authorization.ToString());
            Assert.AreEqual($"{_settings.ApiUrl}", _httpClient.BaseAddress.ToString());
            Assert.AreEqual(HttpMethod.Get, _httpClient.Requests[0].Method);
            Assert.AreEqual(expectedRoute, _httpClient.Requests[0].Uri);

            Assert.IsNull(_httpClient.Requests[0].Content);
            Assert.IsNotNull(retrieved);
            Assert.AreEqual(location.Id, retrieved.Id);
            Assert.AreEqual(location.Name, retrieved.Name);
        }

        [TestMethod]
        public async Task ListTest()
        {
            var location = DataGenerator.CreateLocation();
            var json = JsonSerializer.Serialize<List<Location>>([location]);
            _httpClient.AddResponse(json);

            var locations = await _client.ListAsync(1, int.MaxValue);
            var expectedRoute = $"{_settings.ApiRoutes.First(x => x.Name == "Location").Route}/1/{int.MaxValue}";

            Assert.AreEqual($"Bearer {ApiToken}", _httpClient.DefaultRequestHeaders.Authorization.ToString());
            Assert.AreEqual($"{_settings.ApiUrl}", _httpClient.BaseAddress.ToString());
            Assert.AreEqual(HttpMethod.Get, _httpClient.Requests[0].Method);
            Assert.AreEqual(expectedRoute, _httpClient.Requests[0].Uri);

            Assert.IsNull(_httpClient.Requests[0].Content);
            Assert.IsNotNull(locations);
            Assert.HasCount(1, locations);
            Assert.AreEqual(location.Id, locations[0].Id);
            Assert.AreEqual(location.Name, locations[0].Name);
        }

        [TestMethod]
        public async Task ImportFromFileTest()
        {
            var location = DataGenerator.CreateLocation();
            _filePath = Path.ChangeExtension(Path.GetTempFileName(), "csv");
            File.WriteAllLines(_filePath, ["", $"""{location.Name}"""]);
            _httpClient.AddResponse("");

            var content = File.ReadAllText(_filePath);
            var json = JsonSerializer.Serialize(new { Content = content });
            var expectedRoute = _settings.ApiRoutes.First(x => x.Name == "ImportLocation").Route;

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
            var expectedRoute = _settings.ApiRoutes.First(x => x.Name == "ExportLocation").Route;

            await _client.ExportAsync(_filePath);

            Assert.AreEqual($"Bearer {ApiToken}", _httpClient.DefaultRequestHeaders.Authorization.ToString());
            Assert.AreEqual($"{_settings.ApiUrl}", _httpClient.BaseAddress.ToString());
            Assert.AreEqual(HttpMethod.Post, _httpClient.Requests[0].Method);
            Assert.AreEqual(expectedRoute, _httpClient.Requests[0].Uri);
            Assert.AreEqual(json, await _httpClient.Requests[0].Content.ReadAsStringAsync());
        }
    }
}
