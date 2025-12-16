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
    public class CountryClient : ShippingRecorderClientBase, ICountryClient
    {
        private const string RouteKey = "Country";

        public CountryClient(
            IShippingRecorderHttpClient client,
            IShippingRecorderApplicationSettings settings,
            IAuthenticationTokenProvider tokenProvider,
            ICacheWrapper cache,
            ILogger<CountryClient> logger)
            : base(client, settings, tokenProvider, cache, logger)
        {
        }

        /// <summary>
        /// Return the country with the specified ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<Country> GetAsync(long id)
        {
            var baseRoute = Settings.ApiRoutes.First(r => r.Name == RouteKey).Route;
            var route = $"{baseRoute}/{id}";
            var json = await SendDirectAsync(route, null, HttpMethod.Get);
            var country = Deserialize<Country>(json);
            return country;
        }

        /// <summary>
        /// Add a new country to the database
        /// </summary>
        /// <param
        /// <param name="code"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public async Task<Country> AddAsync(string code, string name)
        {
            dynamic template = new
            {
                Code = code,
                Name = name
            };

            var data = Serialize(template);
            string json = await SendIndirectAsync(RouteKey, data, HttpMethod.Post);
            var country = Deserialize<Country>(json);

            return country;
        }

        /// <summary>
        /// Update an existing country
        /// </summary>
        /// <param name="id"></param>
        /// <param name="code"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public async Task<Country> UpdateAsync(long id, string code, string name)
        {
            dynamic template = new
            {
                Id = id,
                Code = code,
                Name = name
            };

            var data = Serialize(template);
            string json = await SendIndirectAsync(RouteKey, data, HttpMethod.Put);
            var country = Deserialize<Country>(json);

            return country;
        }

        /// <summary>
        /// Delete a country from the database
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
        /// Return a list of countries
        /// </summary>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public async Task<List<Country>> ListAsync(int pageNumber, int pageSize)
        {
            // Request a list of countries
            string baseRoute = @$"{Settings.ApiRoutes.First(r => r.Name == RouteKey).Route}";
            var route = $"{baseRoute}/{pageNumber}/{pageSize}";
            string json = await SendDirectAsync(route, null, HttpMethod.Get);

            // The returned JSON will be empty if there are no countries in the database
            List<Country> countries = Deserialize<List<Country>>(json);
            return countries;
        }
    }
}
