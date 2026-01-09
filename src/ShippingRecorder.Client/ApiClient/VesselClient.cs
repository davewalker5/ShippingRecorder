using ShippingRecorder.Client.Interfaces;
using ShippingRecorder.Entities.Interfaces;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using ShippingRecorder.Entities.Db;
using System.Net.Http;
using System.Linq;
using System.Collections.Generic;
using System.IO;

namespace ShippingRecorder.Client.ApiClient
{
    public class VesselClient : ShippingRecorderClientBase, IVesselClient
    {
        private const string RouteKey = "Vessel";
        private const string ImportRouteKey = "ImportVessel";

        public VesselClient(
            IShippingRecorderHttpClient client,
            IShippingRecorderApplicationSettings settings,
            IAuthenticationTokenProvider tokenProvider,
            ICacheWrapper cache,
            ILogger<VesselClient> logger)
            : base(client, settings, tokenProvider, cache, logger)
        {
        }

        /// <summary>
        /// Return the vessel with the specified ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<Vessel> GetAsync(long id)
        {
            var baseRoute = Settings.ApiRoutes.First(r => r.Name == RouteKey).Route;
            var route = $"{baseRoute}/{id}";
            var json = await SendDirectAsync(route, null, HttpMethod.Get);
            var vessel = Deserialize<Vessel>(json);
            return vessel;
        }

        /// <summary>
        /// Return the vessel with the specified IMO
        /// </summary>
        /// <param name="imo"></param>
        /// <returns></returns>
        public async Task<Vessel> GetAsync(string imo)
        {
            var baseRoute = Settings.ApiRoutes.First(r => r.Name == RouteKey).Route;
            var route = $"{baseRoute}/imo/{imo}";
            var json = await SendDirectAsync(route, null, HttpMethod.Get);
            var vessel = Deserialize<Vessel>(json);
            return vessel;
        }

        /// <summary>
        /// Add a new vessel to the database
        /// </summary>
        /// <param name="imo"></param>
        /// <param name="built"></param>
        /// <param name="draught"></param>
        /// <param name="length"></param>
        /// <param name="beam"></param>
        /// <returns></returns>
        public async Task<Vessel> AddAsync(string imo, int? built, decimal? draught, int? length, int? beam)
        {
            dynamic template = new
            {
                IMO = imo,
                Built = built,
                Draught = draught,
                Length = length,
                Beam = beam
            };

            var data = Serialize(template);
            string json = await SendIndirectAsync(RouteKey, data, HttpMethod.Post);
            var vessel = Deserialize<Vessel>(json);

            return vessel;
        }

        /// <summary>
        /// Update an existing vessel
        /// </summary>
        /// <param name="id"></param>
        /// <param name="imo"></param>
        /// <param name="built"></param>
        /// <param name="draught"></param>
        /// <param name="length"></param>
        /// <param name="beam"></param>
        /// <returns></returns>
        public async Task<Vessel> UpdateAsync(long id, string imo, int? built, decimal? draught, int? length, int? beam)
        {
            dynamic template = new
            {
                Id = id,
                IMO = imo,
                Built = built,
                Draught = draught,
                Length = length,
                Beam = beam
            };

            var data = Serialize(template);
            string json = await SendIndirectAsync(RouteKey, data, HttpMethod.Put);
            var vessel = Deserialize<Vessel>(json);

            return vessel;
        }

        /// <summary>
        /// Delete a vessel from the database
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
        /// Return a list of vessels
        /// </summary>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public async Task<List<Vessel>> ListAsync(int pageNumber, int pageSize)
        {
            // Request a list of countries
            string baseRoute = @$"{Settings.ApiRoutes.First(r => r.Name == RouteKey).Route}";
            var route = $"{baseRoute}/{pageNumber}/{pageSize}";
            string json = await SendDirectAsync(route, null, HttpMethod.Get);

            // The returned JSON will be empty if there are no countries in the database
            List<Vessel> vessels = Deserialize<List<Vessel>>(json);
            return vessels;
        }

        /// <summary>
        /// Request an import of vessels from the content of a file
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
        /// Request an import of vessels given the path to a file
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public async Task ImportFromFileAsync(string filePath)
            => await ImportFromFileContentAsync(File.ReadAllText(filePath));
    }
}
