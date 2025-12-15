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
    public class OperatorClient : ShippingRecorderClientBase, IOperatorClient
    {
        private const string RouteKey = "Operator";

        public OperatorClient(
            IShippingRecorderHttpClient client,
            IShippingRecorderApplicationSettings settings,
            IAuthenticationTokenProvider tokenProvider,
            ICacheWrapper cache,
            ILogger<OperatorClient> logger)
            : base(client, settings, tokenProvider, cache, logger)
        {
        }

        /// <summary>
        /// Add a new operator to the database
        /// </summary>
        /// <param
        /// <param name="name"></param>
        /// <returns></returns>
        public async Task<Operator> AddAsync(string name)
        {
            dynamic template = new
            {
                Name = name
            };

            var data = Serialize(template);
            string json = await SendIndirectAsync(RouteKey, data, HttpMethod.Post);
            var op = Deserialize<Operator>(json);

            return op;
        }

        /// <summary>
        /// Update an existing operator
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public async Task<Operator> UpdateAsync(long id, string name)
        {
            dynamic template = new
            {
                Id = id,
                Name = name
            };

            var data = Serialize(template);
            string json = await SendIndirectAsync(RouteKey, data, HttpMethod.Put);
            var op = Deserialize<Operator>(json);

            return op;
        }

        /// <summary>
        /// Delete a operator from the database
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
        /// Return a list of operators
        /// </summary>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public async Task<List<Operator>> ListAsync(int pageNumber, int pageSize)
        {
            // Request a list of operators
            string baseRoute = @$"{Settings.ApiRoutes.First(r => r.Name == RouteKey).Route}";
            var route = $"{baseRoute}/{pageNumber}/{pageSize}";
            string json = await SendDirectAsync(route, null, HttpMethod.Get);

            // The returned JSON will be empty if there are no operators in the database
            List<Operator> operators = Deserialize<List<Operator>>(json);
            return operators;
        }
    }
}
