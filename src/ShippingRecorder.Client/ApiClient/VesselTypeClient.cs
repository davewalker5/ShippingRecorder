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
    public class VesselTypeClient : ShippingRecorderClientBase, IVesselTypeClient
    {
        private const string RouteKey = "VesselType";

        public VesselTypeClient(
            IShippingRecorderHttpClient client,
            IShippingRecorderApplicationSettings settings,
            IAuthenticationTokenProvider tokenProvider,
            ILogger<VesselTypeClient> logger)
            : base(client, settings, tokenProvider, logger)
        {
        }

        /// <summary>
        /// Add a new vessel type to the database
        /// </summary>
        /// <param
        /// <param name="name"></param>
        /// <returns></returns>
        public async Task<VesselType> AddAsync(string name)
        {
            dynamic template = new
            {
                Name = name
            };

            var data = Serialize(template);
            string json = await SendIndirectAsync(RouteKey, data, HttpMethod.Post);
            var vesseltype = Deserialize<VesselType>(json);

            return vesseltype;
        }

        /// <summary>
        /// Update an existing vessel type
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public async Task<VesselType> UpdateAsync(long id, string name)
        {
            dynamic template = new
            {
                Id = id,
                Name = name
            };

            var data = Serialize(template);
            string json = await SendIndirectAsync(RouteKey, data, HttpMethod.Put);
            var vesseltype = Deserialize<VesselType>(json);

            return vesseltype;
        }

        /// <summary>
        /// Delete a vessel type from the database
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
        /// Return a list of vessel types
        /// </summary>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public async Task<List<VesselType>> ListAsync(int pageNumber, int pageSize)
        {
            // Request a list of vessel types
            string baseRoute = @$"{Settings.ApiRoutes.First(r => r.Name == RouteKey).Route}";
            var route = $"{baseRoute}/{pageNumber}/{pageSize}";
            string json = await SendDirectAsync(route, null, HttpMethod.Get);

            // The returned JSON will be empty if there are no vessel types in the database
            List<VesselType> vesseltypes = Deserialize<List<VesselType>>(json);
            return vesseltypes;
        }
    }
}
