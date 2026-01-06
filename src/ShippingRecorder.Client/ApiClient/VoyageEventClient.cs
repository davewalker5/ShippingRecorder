using ShippingRecorder.Client.Interfaces;
using ShippingRecorder.Entities.Interfaces;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using ShippingRecorder.Entities.Db;
using System.Net.Http;
using System.Linq;
using System.Collections.Generic;
using System;

namespace ShippingRecorder.Client.ApiClient
{
    public class VoyageEventClient : ShippingRecorderClientBase, IVoyageEventClient
    {
        private const string RouteKey = "VoyageEvent";

        public VoyageEventClient(
            IShippingRecorderHttpClient client,
            IShippingRecorderApplicationSettings settings,
            IAuthenticationTokenProvider tokenProvider,
            ICacheWrapper cache,
            ILogger<VoyageEventClient> logger)
            : base(client, settings, tokenProvider, cache, logger)
        {
        }

        /// <summary>
        /// Add a new voyage event to the database
        /// </summary>
        /// <param
        /// <param name="voyageId"></param>
        /// <param name="eventType"></param>
        /// <param name="portId"></param>
        /// <param name="date"></param>
        /// <returns></returns>
        public async Task<VoyageEvent> AddAsync(long voyageId, VoyageEventType eventType, long portId, DateTime date)
        {
            dynamic template = new
            {
                VoyageId = voyageId,
                EventType = eventType,
                PortId = portId,
                Date = date
            };

            var data = Serialize(template);
            string json = await SendIndirectAsync(RouteKey, data, HttpMethod.Post);
            var voyageevent = Deserialize<VoyageEvent>(json);

            return voyageevent;
        }

        /// <summary>
        /// Update an existing voyage event
        /// </summary>
        /// <param name="id"></param>
        /// <param name="voyageId"></param>
        /// <param name="eventType"></param>
        /// <param name="portId"></param>
        /// <param name="date"></param>
        /// <returns></returns>
        public async Task<VoyageEvent> UpdateAsync(long id, long voyageId, VoyageEventType eventType, long portId, DateTime date)
        {
            dynamic template = new
            {
                Id = id,
                VoyageId = voyageId,
                EventType = eventType,
                PortId = portId,
                Date = date
            };

            var data = Serialize(template);
            string json = await SendIndirectAsync(RouteKey, data, HttpMethod.Put);
            var voyageevent = Deserialize<VoyageEvent>(json);

            return voyageevent;
        }

        /// <summary>
        /// Delete a voyage event from the database
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
        /// Return a list of voyage events
        /// </summary>
        /// <param name="voyageId"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public async Task<List<VoyageEvent>> ListAsync(long voyageId, int pageNumber, int pageSize)
        {
            // Request a list of voyage events
            string baseRoute = @$"{Settings.ApiRoutes.First(r => r.Name == RouteKey).Route}";
            var route = $"{baseRoute}/{voyageId}/{pageNumber}/{pageSize}";
            string json = await SendDirectAsync(route, null, HttpMethod.Get);

            // The returned JSON will be empty if there are no voyage events in the database
            List<VoyageEvent> countries = Deserialize<List<VoyageEvent>>(json);
            return countries;
        }
    }
}
