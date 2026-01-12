using ShippingRecorder.Client.Interfaces;
using ShippingRecorder.Entities.Interfaces;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using ShippingRecorder.Entities.Db;
using System.Net.Http;
using System.Linq;
using System.Collections.Generic;

namespace ShippingRecorder.Client.ApiClient
{
    public class VoyageClient : ShippingRecorderClientBase, IVoyageClient
    {
        private const string RouteKey = "Voyage";

        public VoyageClient(
            IShippingRecorderHttpClient client,
            IShippingRecorderApplicationSettings settings,
            IAuthenticationTokenProvider tokenProvider,
            ICacheWrapper cache,
            ILogger<VoyageClient> logger)
            : base(client, settings, tokenProvider, cache, logger)
        {
        }

        /// <summary>
        /// Return the voyage with the specified ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<Voyage> GetAsync(long id)
        {
            var baseRoute = Settings.ApiRoutes.First(r => r.Name == RouteKey).Route;
            var route = $"{baseRoute}/{id}";
            var json = await SendDirectAsync(route, null, HttpMethod.Get);
            var voyage = Deserialize<Voyage>(json);
            return voyage;
        }

        /// <summary>
        /// Add a new voyage to the database
        /// </summary>
        /// <param
        /// <param name="operatorId"></param>
        /// <param name="vesselId"></param>
        /// <param name="number"></param>
        /// <returns></returns>
        public async Task<Voyage> AddAsync(long operatorId, long vesselId, string number)
        {
            dynamic template = new
            {
                OperatorId = operatorId,
                VesselId = vesselId,
                Number = number
            };

            var data = Serialize(template);
            string json = await SendIndirectAsync(RouteKey, data, HttpMethod.Post);
            var voyage = Deserialize<Voyage>(json);

            return voyage;
        }

        /// <summary>
        /// Update an existing voyage
        /// </summary>
        /// <param name="id"></param>
        /// <param name="operatorId"></param>
        /// <param name="vesselId"></param>
        /// <param name="number"></param>
        /// <returns></returns>
        public async Task<Voyage> UpdateAsync(long id, long operatorId, long vesselId, string number)
        {
            dynamic template = new
            {
                Id = id,
                OperatorId = operatorId,
                VesselId = vesselId,
                Number = number
            };

            var data = Serialize(template);
            string json = await SendIndirectAsync(RouteKey, data, HttpMethod.Put);
            var voyage = Deserialize<Voyage>(json);

            return voyage;
        }

        /// <summary>
        /// Delete a voyage from the database
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
        /// Return a list of voyages
        /// </summary>
        /// <param name="operatorId"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public async Task<List<Voyage>> ListAsync(long operatorId, int pageNumber, int pageSize)
        {
            // Request a list of voyages
            string baseRoute = @$"{Settings.ApiRoutes.First(r => r.Name == RouteKey).Route}";
            var route = $"{baseRoute}/{operatorId}/{pageNumber}/{pageSize}";
            string json = await SendDirectAsync(route, null, HttpMethod.Get);

            // The returned JSON will be empty if there are no voyages in the database
            List<Voyage> countries = Deserialize<List<Voyage>>(json);
            return countries;
        }
    }
}
