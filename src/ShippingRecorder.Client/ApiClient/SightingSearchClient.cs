using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using ShippingRecorder.Client.Interfaces;
using ShippingRecorder.Entities.Db;
using ShippingRecorder.Entities.Interfaces;

namespace ShippingRecorder.Client.ApiClient
{
    public class SightingSearchClient : ShippingRecorderClientBase, ISightingSearchClient
    {
        private const string RouteKey = "Sighting";

        public SightingSearchClient(
            IShippingRecorderHttpClient client,
            IShippingRecorderApplicationSettings settings,
            IAuthenticationTokenProvider tokenProvider,
            ICacheWrapper cache,
            ILogger<SightingSearchClient> logger)
            : base(client, settings, tokenProvider, cache, logger)
        {
        }

        /// <summary>
        /// Get the most recent sighting of an aircraft
        /// </summary>
        /// <param name="imo"></param>
        /// <returns></returns>
        public async Task<Sighting> GetMostRecentVesselSightingAsync(string imo)
        {
            string baseRoute = Settings.ApiRoutes.First(r => r.Name == RouteKey).Route;
            string route = $"{baseRoute}/recent/imo/{imo}";
            string json = await SendDirectAsync(route, null, HttpMethod.Get);
            Sighting sighting = Deserialize<Sighting>(json);
            return sighting;
        }
    }
}