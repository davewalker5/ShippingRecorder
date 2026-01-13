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
    public class PortClient : ShippingRecorderClientBase, IPortClient
    {
        private const string RouteKey = "Port";
        private const string ImportRouteKey = "ImportPort";

        public PortClient(
            IShippingRecorderHttpClient client,
            IShippingRecorderApplicationSettings settings,
            IAuthenticationTokenProvider tokenProvider,
            ICacheWrapper cache,
            ILogger<PortClient> logger)
            : base(client, settings, tokenProvider, cache, logger)
        {
        }

        /// <summary>
        /// Return a port given a UN/LOCODE
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public async Task<Port> GetAsync(string code)
        {
            // Request the specified port
            string baseRoute = @$"{Settings.ApiRoutes.First(r => r.Name == RouteKey).Route}";
            var route = $"{baseRoute}/unlocode/{code}";
            string json = await SendDirectAsync(route, null, HttpMethod.Get);

            // TODO:
            Port port = Deserialize<Port>(json);
            return port;
        }
        

        /// <summary>
        /// Add a new port to the database
        /// </summary>
        /// <param
        /// <param name="countryId"></param>
        /// <param name="code"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public async Task<Port> AddAsync(long countryId, string code, string name)
        {
            dynamic template = new
            {
                CountryId = countryId,
                Code = code,
                Name = name
            };

            var data = Serialize(template);
            string json = await SendIndirectAsync(RouteKey, data, HttpMethod.Post);
            var port = Deserialize<Port>(json);

            return port;
        }

        /// <summary>
        /// Update an existing port
        /// </summary>
        /// <param name="id"></param>
        /// <param name="countryId"></param>
        /// <param name="code"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public async Task<Port> UpdateAsync(long id, long countryId, string code, string name)
        {
            dynamic template = new
            {
                Id = id,
                CountryId = countryId,
                Code = code,
                Name = name
            };

            var data = Serialize(template);
            string json = await SendIndirectAsync(RouteKey, data, HttpMethod.Put);
            var port = Deserialize<Port>(json);

            return port;
        }

        /// <summary>
        /// Delete a port from the database
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
        /// Return a list of ports
        /// </summary>
        /// <param name="countryId"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public async Task<List<Port>> ListAsync(long countryId, int pageNumber, int pageSize)
        {
            // Request a list of countries
            string baseRoute = @$"{Settings.ApiRoutes.First(r => r.Name == RouteKey).Route}";
            var route = $"{baseRoute}/{countryId}/{pageNumber}/{pageSize}";
            string json = await SendDirectAsync(route, null, HttpMethod.Get);

            // The returned JSON will be empty if there are no countries in the database
            List<Port> ports = Deserialize<List<Port>>(json);
            return ports;
        }

        /// <summary>
        /// Request an import of ports from the content of a file
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
        /// Request an import of ports given the path to a file
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public async Task ImportFromFileAsync(string filePath)
            => await ImportFromFileContentAsync(File.ReadAllText(filePath));
    }
}
