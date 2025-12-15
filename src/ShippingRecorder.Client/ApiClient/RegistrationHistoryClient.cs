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
    public class RegistrationHistoryClient : ShippingRecorderClientBase, IRegistrationHistoryClient
    {
        private const string RouteKey = "RegistrationHistory";

        public RegistrationHistoryClient(
            IShippingRecorderHttpClient client,
            IShippingRecorderApplicationSettings settings,
            IAuthenticationTokenProvider tokenProvider,
            ICacheWrapper cache,
            ILogger<RegistrationHistoryClient> logger)
            : base(client, settings, tokenProvider, cache, logger)
        {
        }

        /// <summary>
        /// Add a new registration history to the database
        /// </summary>
        /// <param name="vesselId"></param>
        /// <param name="vesselTypeId"></param>
        /// <param name="flagId"></param>
        /// <param name="operatorId"></param>
        /// <param name="date"></param>
        /// <param name="name"></param>
        /// <param name="callsign"></param>
        /// <param name="mmsi"></param>
        /// <param name="tonnage"></param>
        /// <param name="passengers"></param>
        /// <param name="crew"></param>
        /// <param name="decks"></param>
        /// <param name="cabins"></param>
        /// <returns></returns>
        public async Task<RegistrationHistory> AddAsync(
            long vesselId,
            long vesselTypeId,
            long flagId,
            long operatorId,
            DateTime date,
            string name,
            string callsign,
            string mmsi,
            int? tonnage,
            int? passengers,
            int? crew,
            int? decks,
            int? cabins)
        {
            dynamic template = new
            {
                VesselId = vesselId,
                VesselTypeId = vesselTypeId,
                FlagId = flagId,
                OperatorId = operatorId,
                Date = date,
                Name = name,
                Callsign = callsign,
                MMSI = mmsi,
                Tonnage = tonnage,
                Passengers = passengers,
                Crew = crew,
                Decks = decks,
                Cabins = cabins
            };

            var data = Serialize(template);
            string json = await SendIndirectAsync(RouteKey, data, HttpMethod.Post);
            var location = Deserialize<RegistrationHistory>(json);

            return location;
        }

        /// <summary>
        /// Deactivate a registration history
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task Deactivate(long id)
        {
            var baseRoute = Settings.ApiRoutes.First(r => r.Name == RouteKey).Route;
            var route = $"{baseRoute}/{id}";
            _ = await SendDirectAsync(route, null, HttpMethod.Delete);
        }

        /// <summary>
        /// Return a list of registration histories
        /// </summary>
        /// <param name="vesselId"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public async Task<List<RegistrationHistory>> ListAsync(long vesselId, int pageNumber, int pageSize)
        {
            // Request a list of registration histories
            string baseRoute = @$"{Settings.ApiRoutes.First(r => r.Name == RouteKey).Route}";
            var route = $"{baseRoute}/{vesselId}/{pageNumber}/{pageSize}";
            string json = await SendDirectAsync(route, null, HttpMethod.Get);

            // The returned JSON will be empty if there are no registration histories in the database
            List<RegistrationHistory> locations = Deserialize<List<RegistrationHistory>>(json);
            return locations;
        }
    }
}
