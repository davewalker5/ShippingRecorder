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
using System;
using ShippingRecorder.Entities.Reporting;
using System.Web;

namespace ShippingRecorder.Tests.Client
{
    [TestClass]
    public class ReportsClientTest
    {
        private readonly string ApiToken = "An API Token";
        private readonly MockShippingRecorderHttpClient _httpClient = new();
        private IReportsClient _client;

        private readonly ShippingRecorderApplicationSettings _settings = new()
        {
            ApiUrl = "http://server/",
            DateTimeFormat = "yyyy-MM-dd H:mm:ss",
            ApiRoutes = [
                new() { Name = "JobStatus", Route = "/reports/jobs" },
                new() { Name = "LocationStatistics", Route = "/reports/locations" },
                new() { Name = "SightingsByMonth", Route = "/reports/sightings" },
                new() { Name = "OperatorStatistics", Route = "/reports/operators" },
                new() { Name = "VesselTypeStatistics", Route = "/reports/vesseltypes" },
                new() { Name = "FlagStatistics", Route = "/reports/flags" },
                new() { Name = "MyVoyages", Route = "/reports/myvoyages" }
            ]
        };

        [TestInitialize]
        public void Initialise()
        {
            var provider = new Mock<IAuthenticationTokenProvider>();
            provider.Setup(x => x.GetToken()).Returns(ApiToken);
            var logger = new Mock<ILogger<ReportsClient>>();
            var cache = new Mock<ICacheWrapper>();
            _client = new ReportsClient(_httpClient, _settings, provider.Object, cache.Object, logger.Object);
        }

        [TestMethod]
        public async Task JobStatusReportTest()
        {
            var from = DateTime.Today.AddDays(-DataGenerator.RandomInt(1, 30));
            var to  = DateTime.Today.AddDays(DataGenerator.RandomInt(1, 30));
            var json = JsonSerializer.Serialize<List<JobStatus>>([new JobStatus()]);
            _httpClient.AddResponse(json);

            string fromRouteSegment = HttpUtility.UrlEncode(from.ToString(_settings.DateTimeFormat));
            string toRouteSegment = HttpUtility.UrlEncode(to.ToString(_settings.DateTimeFormat));

            var report = await _client.JobStatusAsync(from, to, 1, int.MaxValue);

            var expectedRoute = $"{_settings.ApiRoutes.First(x => x.Name == "JobStatus").Route}/{fromRouteSegment}/{toRouteSegment}/1/{int.MaxValue}";

            Assert.AreEqual($"Bearer {ApiToken}", _httpClient.DefaultRequestHeaders.Authorization.ToString());
            Assert.AreEqual($"{_settings.ApiUrl}", _httpClient.BaseAddress.ToString());
            Assert.AreEqual(HttpMethod.Get, _httpClient.Requests[0].Method);
            Assert.AreEqual(expectedRoute, _httpClient.Requests[0].Uri);
        }

        [TestMethod]
        public async Task LocationStatisticsTest()
        {
            var from = DateTime.Today.AddDays(-DataGenerator.RandomInt(1, 30));
            var to  = DateTime.Today.AddDays(DataGenerator.RandomInt(1, 30));
            var json = JsonSerializer.Serialize<List<LocationStatistics>>([new LocationStatistics()]);
            _httpClient.AddResponse(json);

            string fromRouteSegment = HttpUtility.UrlEncode(from.ToString(_settings.DateTimeFormat));
            string toRouteSegment = HttpUtility.UrlEncode(to.ToString(_settings.DateTimeFormat));

            var report = await _client.LocationStatisticsAsync(from, to, 1, int.MaxValue);

            var expectedRoute = $"{_settings.ApiRoutes.First(x => x.Name == "LocationStatistics").Route}/{fromRouteSegment}/{toRouteSegment}/1/{int.MaxValue}";

            Assert.AreEqual($"Bearer {ApiToken}", _httpClient.DefaultRequestHeaders.Authorization.ToString());
            Assert.AreEqual($"{_settings.ApiUrl}", _httpClient.BaseAddress.ToString());
            Assert.AreEqual(HttpMethod.Get, _httpClient.Requests[0].Method);
            Assert.AreEqual(expectedRoute, _httpClient.Requests[0].Uri);
        }

        [TestMethod]
        public async Task SightingsByMonthTest()
        {
            var from = DateTime.Today.AddDays(-DataGenerator.RandomInt(1, 30));
            var to  = DateTime.Today.AddDays(DataGenerator.RandomInt(1, 30));
            var json = JsonSerializer.Serialize<List<SightingsByMonth>>([new SightingsByMonth()]);
            _httpClient.AddResponse(json);

            string fromRouteSegment = HttpUtility.UrlEncode(from.ToString(_settings.DateTimeFormat));
            string toRouteSegment = HttpUtility.UrlEncode(to.ToString(_settings.DateTimeFormat));

            var report = await _client.SightingsByMonthAsync(from, to, 1, int.MaxValue);

            var expectedRoute = $"{_settings.ApiRoutes.First(x => x.Name == "SightingsByMonth").Route}/{fromRouteSegment}/{toRouteSegment}/1/{int.MaxValue}";

            Assert.AreEqual($"Bearer {ApiToken}", _httpClient.DefaultRequestHeaders.Authorization.ToString());
            Assert.AreEqual($"{_settings.ApiUrl}", _httpClient.BaseAddress.ToString());
            Assert.AreEqual(HttpMethod.Get, _httpClient.Requests[0].Method);
            Assert.AreEqual(expectedRoute, _httpClient.Requests[0].Uri);
        }

        [TestMethod]
        public async Task OperatorStatisticsTest()
        {
            var from = DateTime.Today.AddDays(-DataGenerator.RandomInt(1, 30));
            var to  = DateTime.Today.AddDays(DataGenerator.RandomInt(1, 30));
            var json = JsonSerializer.Serialize<List<OperatorStatistics>>([new OperatorStatistics()]);
            _httpClient.AddResponse(json);

            string fromRouteSegment = HttpUtility.UrlEncode(from.ToString(_settings.DateTimeFormat));
            string toRouteSegment = HttpUtility.UrlEncode(to.ToString(_settings.DateTimeFormat));

            var report = await _client.OperatorStatisticsAsync(from, to, 1, int.MaxValue);

            var expectedRoute = $"{_settings.ApiRoutes.First(x => x.Name == "OperatorStatistics").Route}/{fromRouteSegment}/{toRouteSegment}/1/{int.MaxValue}";

            Assert.AreEqual($"Bearer {ApiToken}", _httpClient.DefaultRequestHeaders.Authorization.ToString());
            Assert.AreEqual($"{_settings.ApiUrl}", _httpClient.BaseAddress.ToString());
            Assert.AreEqual(HttpMethod.Get, _httpClient.Requests[0].Method);
            Assert.AreEqual(expectedRoute, _httpClient.Requests[0].Uri);
        }

        [TestMethod]
        public async Task VesselTypeStatisticsTest()
        {
            var from = DateTime.Today.AddDays(-DataGenerator.RandomInt(1, 30));
            var to  = DateTime.Today.AddDays(DataGenerator.RandomInt(1, 30));
            var json = JsonSerializer.Serialize<List<VesselTypeStatistics>>([new VesselTypeStatistics()]);
            _httpClient.AddResponse(json);

            string fromRouteSegment = HttpUtility.UrlEncode(from.ToString(_settings.DateTimeFormat));
            string toRouteSegment = HttpUtility.UrlEncode(to.ToString(_settings.DateTimeFormat));

            var report = await _client.VesselTypeStatisticsAsync(from, to, 1, int.MaxValue);

            var expectedRoute = $"{_settings.ApiRoutes.First(x => x.Name == "VesselTypeStatistics").Route}/{fromRouteSegment}/{toRouteSegment}/1/{int.MaxValue}";

            Assert.AreEqual($"Bearer {ApiToken}", _httpClient.DefaultRequestHeaders.Authorization.ToString());
            Assert.AreEqual($"{_settings.ApiUrl}", _httpClient.BaseAddress.ToString());
            Assert.AreEqual(HttpMethod.Get, _httpClient.Requests[0].Method);
            Assert.AreEqual(expectedRoute, _httpClient.Requests[0].Uri);
        }

        [TestMethod]
        public async Task FlagStatisticsTest()
        {
            var from = DateTime.Today.AddDays(-DataGenerator.RandomInt(1, 30));
            var to  = DateTime.Today.AddDays(DataGenerator.RandomInt(1, 30));
            var json = JsonSerializer.Serialize<List<FlagStatistics>>([new FlagStatistics()]);
            _httpClient.AddResponse(json);

            string fromRouteSegment = HttpUtility.UrlEncode(from.ToString(_settings.DateTimeFormat));
            string toRouteSegment = HttpUtility.UrlEncode(to.ToString(_settings.DateTimeFormat));

            var report = await _client.FlagStatisticsAsync(from, to, 1, int.MaxValue);

            var expectedRoute = $"{_settings.ApiRoutes.First(x => x.Name == "FlagStatistics").Route}/{fromRouteSegment}/{toRouteSegment}/1/{int.MaxValue}";

            Assert.AreEqual($"Bearer {ApiToken}", _httpClient.DefaultRequestHeaders.Authorization.ToString());
            Assert.AreEqual($"{_settings.ApiUrl}", _httpClient.BaseAddress.ToString());
            Assert.AreEqual(HttpMethod.Get, _httpClient.Requests[0].Method);
            Assert.AreEqual(expectedRoute, _httpClient.Requests[0].Uri);
        }

        [TestMethod]
        public async Task MyVoyagesTest()
        {
            var from = DateTime.Today.AddDays(-DataGenerator.RandomInt(1, 30));
            var to  = DateTime.Today.AddDays(DataGenerator.RandomInt(1, 30));
            var json = JsonSerializer.Serialize<List<MyVoyages>>([new MyVoyages()]);
            _httpClient.AddResponse(json);

            string fromRouteSegment = HttpUtility.UrlEncode(from.ToString(_settings.DateTimeFormat));
            string toRouteSegment = HttpUtility.UrlEncode(to.ToString(_settings.DateTimeFormat));

            var report = await _client.MyVoyagesAsync(from, to, 1, int.MaxValue);

            var expectedRoute = $"{_settings.ApiRoutes.First(x => x.Name == "MyVoyages").Route}/{fromRouteSegment}/{toRouteSegment}/1/{int.MaxValue}";

            Assert.AreEqual($"Bearer {ApiToken}", _httpClient.DefaultRequestHeaders.Authorization.ToString());
            Assert.AreEqual($"{_settings.ApiUrl}", _httpClient.BaseAddress.ToString());
            Assert.AreEqual(HttpMethod.Get, _httpClient.Requests[0].Method);
            Assert.AreEqual(expectedRoute, _httpClient.Requests[0].Uri);
        }
    }
}
