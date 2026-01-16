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
    public class CountryClientTest
    {
        private readonly string ApiToken = "An API Token";
        private const string FileName = "a-file-name.csv";
        private readonly MockShippingRecorderHttpClient _httpClient = new();
        private ICountryClient _client;
        private string _filePath;

        private readonly ShippingRecorderApplicationSettings _settings = new()
        {
            ApiUrl = "http://server/",
            ApiRoutes = [
                new() { Name = "Country", Route = "/countries" },
                new() { Name = "ImportCountry", Route = "/import/countries" },
                new() { Name = "ExportCountry", Route = "/export/countries" }
            ]
        };

        [TestInitialize]
        public void Initialise()
        {
            var provider = new Mock<IAuthenticationTokenProvider>();
            provider.Setup(x => x.GetToken()).Returns(ApiToken);
            var logger = new Mock<ILogger<CountryClient>>();
            var cache = new Mock<ICacheWrapper>();
            _client = new CountryClient(_httpClient, _settings, provider.Object, cache.Object, logger.Object);
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
            var country = DataGenerator.CreateCountry();
            var json = JsonSerializer.Serialize(new { country.Code, country.Name });
            _httpClient.AddResponse(json);

            var added = await _client.AddAsync(country.Code, country.Name);

            Assert.AreEqual($"Bearer {ApiToken}", _httpClient.DefaultRequestHeaders.Authorization.ToString());
            Assert.AreEqual($"{_settings.ApiUrl}", _httpClient.BaseAddress.ToString());
            Assert.AreEqual(HttpMethod.Post, _httpClient.Requests[0].Method);
            Assert.AreEqual(_settings.ApiRoutes.First(x => x.Name == "Country").Route, _httpClient.Requests[0].Uri);

            Assert.AreEqual(json, await _httpClient.Requests[0].Content.ReadAsStringAsync());
            Assert.IsNotNull(added);
            Assert.AreEqual(country.Code, added.Code);
            Assert.AreEqual(country.Name, added.Name);
        }

        [TestMethod]
        public async Task UpdateTest()
        {
            var country = DataGenerator.CreateCountry();
            var json = JsonSerializer.Serialize(country);
            _httpClient.AddResponse(json);

            var updated = await _client.UpdateAsync(country.Id, country.Code, country.Name);

            Assert.AreEqual($"Bearer {ApiToken}", _httpClient.DefaultRequestHeaders.Authorization.ToString());
            Assert.AreEqual($"{_settings.ApiUrl}", _httpClient.BaseAddress.ToString());
            Assert.AreEqual(HttpMethod.Put, _httpClient.Requests[0].Method);
            Assert.AreEqual(_settings.ApiRoutes.First(x => x.Name == "Country").Route, _httpClient.Requests[0].Uri);

            Assert.AreEqual(json, await _httpClient.Requests[0].Content.ReadAsStringAsync());
            Assert.IsNotNull(updated);
            Assert.AreEqual(country.Id, updated.Id);
            Assert.AreEqual(country.Code, updated.Code);
            Assert.AreEqual(country.Name, updated.Name);
        }

        [TestMethod]
        public async Task DeleteTest()
        {
            var id = DataGenerator.RandomId();
            await _client.DeleteAsync(id);

            Assert.AreEqual($"Bearer {ApiToken}", _httpClient.DefaultRequestHeaders.Authorization.ToString());
            Assert.AreEqual($"{_settings.ApiUrl}", _httpClient.BaseAddress.ToString());
            Assert.AreEqual(HttpMethod.Delete, _httpClient.Requests[0].Method);
            Assert.AreEqual($"{_settings.ApiRoutes.First(x => x.Name == "Country").Route}/{id}", _httpClient.Requests[0].Uri);

            Assert.IsNull(_httpClient.Requests[0].Content);
        }

        [TestMethod]
        public async Task GetTest()
        {
            var country = DataGenerator.CreateCountry();
            var json = JsonSerializer.Serialize(country);
            _httpClient.AddResponse(json);

            var retrieved = await _client.GetAsync(country.Id);
            var expectedRoute = $"{_settings.ApiRoutes.First(x => x.Name == "Country").Route}/{country.Id}";

            Assert.AreEqual($"Bearer {ApiToken}", _httpClient.DefaultRequestHeaders.Authorization.ToString());
            Assert.AreEqual($"{_settings.ApiUrl}", _httpClient.BaseAddress.ToString());
            Assert.AreEqual(HttpMethod.Get, _httpClient.Requests[0].Method);
            Assert.AreEqual(expectedRoute, _httpClient.Requests[0].Uri);

            Assert.IsNull(_httpClient.Requests[0].Content);
            Assert.IsNotNull(retrieved);
            Assert.AreEqual(country.Id, retrieved.Id);
            Assert.AreEqual(country.Code, retrieved.Code);
            Assert.AreEqual(country.Name, retrieved.Name);
        }

        [TestMethod]
        public async Task ListTest()
        {
            var country = DataGenerator.CreateCountry();
            var json = JsonSerializer.Serialize<List<Country>>([country]);
            _httpClient.AddResponse(json);

            var countries = await _client.ListAsync(1, int.MaxValue);
            var expectedRoute = $"{_settings.ApiRoutes.First(x => x.Name == "Country").Route}/1/{int.MaxValue}";

            Assert.AreEqual($"Bearer {ApiToken}", _httpClient.DefaultRequestHeaders.Authorization.ToString());
            Assert.AreEqual($"{_settings.ApiUrl}", _httpClient.BaseAddress.ToString());
            Assert.AreEqual(HttpMethod.Get, _httpClient.Requests[0].Method);
            Assert.AreEqual(expectedRoute, _httpClient.Requests[0].Uri);

            Assert.IsNull(_httpClient.Requests[0].Content);
            Assert.IsNotNull(countries);
            Assert.HasCount(1, countries);
            Assert.AreEqual(country.Id, countries[0].Id);
            Assert.AreEqual(country.Code, countries[0].Code);
            Assert.AreEqual(country.Name, countries[0].Name);
        }

        [TestMethod]
        public async Task ImportFromFileTest()
        {
            var country = DataGenerator.CreateCountry();
            _filePath = Path.ChangeExtension(Path.GetTempFileName(), "csv");
            File.WriteAllLines(_filePath, ["", $"""{country.Code}"",""{country.Name}"""]);
            _httpClient.AddResponse("");

            var content = File.ReadAllText(_filePath);
            var json = JsonSerializer.Serialize(new { Content = content });
            var expectedRoute = _settings.ApiRoutes.First(x => x.Name == "ImportCountry").Route;

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
            var expectedRoute = _settings.ApiRoutes.First(x => x.Name == "ExportCountry").Route;

            await _client.ExportAsync(_filePath);

            Assert.AreEqual($"Bearer {ApiToken}", _httpClient.DefaultRequestHeaders.Authorization.ToString());
            Assert.AreEqual($"{_settings.ApiUrl}", _httpClient.BaseAddress.ToString());
            Assert.AreEqual(HttpMethod.Post, _httpClient.Requests[0].Method);
            Assert.AreEqual(expectedRoute, _httpClient.Requests[0].Uri);
            Assert.AreEqual(json, await _httpClient.Requests[0].Content.ReadAsStringAsync());
        }
    }
}
