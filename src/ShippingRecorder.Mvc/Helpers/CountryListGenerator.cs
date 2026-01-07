using ShippingRecorder.Client.Interfaces;
using ShippingRecorder.Mvc.Interfaces;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace HealthTracker.Mvc.Helpers
{
    public class CountryListGenerator : ICountryListGenerator
    {
        private readonly ICountryClient _client;
        private readonly ILogger<CountryListGenerator> _logger;

        public CountryListGenerator(ICountryClient client, ILogger<CountryListGenerator> logger)
        {
            _client = client;
            _logger = logger;
        }

        /// <summary>
        /// Generate a list of select list items for countries
        /// </summary>
        /// <returns></returns>
        public async Task<IList<SelectListItem>> Create()
        {
            var list = new List<SelectListItem>();

            // Load the list of countries
            var countries = await _client.ListAsync(1, int.MaxValue);
            var count = countries?.Count ?? 0;
            var plural = count == 1 ? "country" : "countries";
            _logger.LogDebug($"{count} {plural} loaded via the service");

            // Create a list of select list items from the list of countries. Add an empty entry if there
            // is more than one. If not, the list will only contain that one country which will be the default
            // selection
            if (count != 1)
            {
                list.Add(new SelectListItem() { Text = "", Value = "0" });
            }

            // Now add an entry for each vessel type
            if (count > 0)
            {
                foreach (var country in countries)
                {
                    list.Add(new SelectListItem() { Text = country.Name, Value = country.Id.ToString() });
                }
            }

            return list;
        }
    }
}