using ShippingRecorder.Client.Interfaces;
using ShippingRecorder.Mvc.Interfaces;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace HealthTracker.Mvc.Helpers
{
    public class LocationListGenerator : ILocationListGenerator
    {
        private readonly ILocationClient _client;
        private readonly ILogger<LocationListGenerator> _logger;

        public LocationListGenerator(ILocationClient client, ILogger<LocationListGenerator> logger)
        {
            _client = client;
            _logger = logger;
        }

        /// <summary>
        /// Generate a list of select list items for locations
        /// </summary>
        /// <returns></returns>
        public async Task<IList<SelectListItem>> Create()
        {
            var list = new List<SelectListItem>();

            // Load the list of locations
            var locations = await _client.ListAsync(1, int.MaxValue);
            var count = locations?.Count ?? 0;
            var plural = count == 1 ? "" : "s";
            _logger.LogDebug($"{count} location{plural} loaded via the service");

            // Create a list of select list items from the list of locations. Add an empty entry if there
            // is more than one. If not, the list will only contain that one location which will be the default
            // selection
            if (count != 1)
            {
                list.Add(new SelectListItem() { Text = "", Value = "0" });
            }

            // Now add an entry for each location
            if (count > 0)
            {
                foreach (var vesselType in locations)
                {
                    list.Add(new SelectListItem() { Text = vesselType.Name, Value = vesselType.Id.ToString() });
                }
            }

            return list;
        }
    }
}