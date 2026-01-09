using ShippingRecorder.Client.Interfaces;
using ShippingRecorder.Entities.Interfaces;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using ShippingRecorder.Entities.Db;
using System.Net.Http;
using System.Linq;
using System.Collections.Generic;
using System;
using System.IO;

namespace ShippingRecorder.Client.ApiClient
{
    public class SightingClient : ShippingRecorderClientBase, ISightingClient
    {
        private const string RouteKey = "Sighting";
        private const string ImportRouteKey = "ImportSighting";

        public SightingClient(
            IShippingRecorderHttpClient client,
            IShippingRecorderApplicationSettings settings,
            IAuthenticationTokenProvider tokenProvider,
            ICacheWrapper cache,
            ILogger<SightingClient> logger)
            : base(client, settings, tokenProvider, cache, logger)
        {
        }

        /// <summary>
        /// Return the sighting with the specified ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<Sighting> GetAsync(long id)
        {
            var baseRoute = Settings.ApiRoutes.First(r => r.Name == RouteKey).Route;
            var route = $"{baseRoute}/{id}";
            var json = await SendDirectAsync(route, null, HttpMethod.Get);
            var sighting = Deserialize<Sighting>(json);
            return sighting;
        }

        /// <summary>
        /// Add a new sighting to the database
        /// </summary>
        /// <param name="locationId"></param>
        /// <param name="voyageId"></param>
        /// <param name="vesselId"></param>
        /// <param name="date"></param>
        /// <param name="isMyVoyage"></param>
        /// <returns></returns>
        public async Task<Sighting> AddAsync(long locationId, long? voyageId, long vesselId, DateTime date, bool isMyVoyage)
        {
            dynamic template = new
            {
                LocationId = locationId,
                VoyageId = voyageId,
                VesselId = vesselId,
                Date = date,
                IsMyVoyage = isMyVoyage
            };

            var data = Serialize(template);
            string json = await SendIndirectAsync(RouteKey, data, HttpMethod.Post);
            var sighting = Deserialize<Sighting>(json);

            return sighting;
        }

        /// <summary>
        /// Update an existing sighting
        /// </summary>
        /// <param name="id"></param>
        /// <param name="locationId"></param>
        /// <param name="voyageId"></param>
        /// <param name="vesselId"></param>
        /// <param name="date"></param>
        /// <param name="isMyVoyage"></param>
        /// <returns></returns>
        public async Task<Sighting> UpdateAsync(long id, long locationId, long? voyageId, long vesselId, DateTime date, bool isMyVoyage)
        {
            dynamic template = new
            {
                Id = id,
                LocationId = locationId,
                VoyageId = voyageId,
                VesselId = vesselId,
                Date = date,
                IsMyVoyage = isMyVoyage
            };

            var data = Serialize(template);
            string json = await SendIndirectAsync(RouteKey, data, HttpMethod.Put);
            var sighting = Deserialize<Sighting>(json);

            return sighting;
        }

        /// <summary>
        /// Delete a sighting from the database
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task DeleteAsync(long id)
        {
            var baseRoute = Settings.ApiRoutes.First(r => r.Name == RouteKey).Route;
            var route = $"{baseRoute}/{id}";
            _ = await SendDirectAsync(route, null, HttpMethod.Delete);
        }

        /// <summary>
        /// Return a list of sightings
        /// </summary>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public async Task<List<Sighting>> ListAsync(int pageNumber, int pageSize)
            => await ListSightingsAsync(null, pageNumber, pageSize);

        /// <summary>
        /// Get the most recent sighting of an aircraft
        /// </summary>
        /// <param name="imo"></param>
        /// <returns></returns>
        public async Task<Sighting> GetMostRecentVesselSightingAsync(string imo)
            => (await ListSightingsAsync($"vessel/{imo}", 1, 1))?.FirstOrDefault();

        /// <summary>
        /// Return a list of sightings for the specified vessel
        /// </summary>
        /// <param name="vesselId"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public async Task<List<Sighting>> ListSightingsByVesselAsync(long vesselId, int pageNumber, int pageSize)
            => await ListSightingsAsync($"vessel/{vesselId}", pageNumber, pageSize);

        /// <summary>
        /// Return a list of sightings at the specified location
        /// </summary>
        /// <param name="locationId"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public async Task<List<Sighting>> ListSightingsByLocationAsync(long locationId, int pageNumber, int pageSize)
        {
            // Determine the encoded date range
            (var encodedFromDate, var encodedToDate) = CalculateEncodedDateRange(DateTime.MinValue, DateTime.Now);
            var sightings = await ListSightingsAsync($"location/{locationId}/{encodedFromDate}/{encodedToDate}", pageNumber, pageSize);
            return sightings;
        }

        /// <summary>
        /// Return a list of sightings in the specified date range
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public async Task<List<Sighting>> ListSightingsByDateAsync(DateTime start, DateTime end, int pageNumber, int pageSize)
        {
            // Determine the encoded date range
            (var encodedFromDate, var encodedToDate) = CalculateEncodedDateRange(start, end);
            var sightings = await ListSightingsAsync($"date/{encodedFromDate}/{encodedToDate}", pageNumber, pageSize);
            return sightings;
        }

        /// <summary>
        /// Return a list of sightings with the "my voyage" flag set
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public async Task<List<Sighting>> ListMyVoyagesAsync(int pageNumber, int pageSize)
            => await ListSightingsAsync($"mine", pageNumber, pageSize);

        /// <summary>
        /// Return a list of sightings from the endpoint with the specified route prefix
        /// </summary>
        /// <param name="prefix"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        private async Task<List<Sighting>> ListSightingsAsync(string prefix, int pageNumber ,int pageSize)
        {
            // Request a list of sightings
            string baseRoute = @$"{Settings.ApiRoutes.First(r => r.Name == RouteKey).Route}";
            var route = string.IsNullOrEmpty(prefix) ?
                $"{baseRoute}/{pageNumber}/{pageSize}" :
                $"{baseRoute}/{prefix}/{pageNumber}/{pageSize}";
            string json = await SendDirectAsync(route, null, HttpMethod.Get);

            // The returned JSON will be empty if there are no sightings in the database
            List<Sighting> sightings = Deserialize<List<Sighting>>(json);
            return sightings;
        }

        /// <summary>
        /// Request an import of sightings from the content of a file
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public async Task ImportFromFileContentAsync(string content)
        {
            dynamic data = new{ Content = content };
            var json = Serialize(data);
            await SendIndirectAsync(ImportRouteKey, json, HttpMethod.Post);
        }

        /// <summary>
        /// Request an import of sightings given the path to a file
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public async Task ImportFromFileAsync(string filePath)
            => await ImportFromFileContentAsync(File.ReadAllText(filePath));
    }
}
