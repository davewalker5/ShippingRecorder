using ShippingRecorder.Client.Interfaces;
using ShippingRecorder.Mvc.Interfaces;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace HealthTracker.Mvc.Helpers
{
    public class VesselTypeListGenerator : IVesselTypeListGenerator
    {
        private readonly IVesselTypeClient _client;
        private readonly ILogger<VesselTypeListGenerator> _logger;

        public VesselTypeListGenerator(IVesselTypeClient client, ILogger<VesselTypeListGenerator> logger)
        {
            _client = client;
            _logger = logger;
        }

        /// <summary>
        /// Generate a list of select list items for vessel types
        /// </summary>
        /// <returns></returns>
        public async Task<IList<SelectListItem>> Create()
        {
            var list = new List<SelectListItem>();

            // Load the list of vessel types
            var vesselTypes = await _client.ListAsync(1, int.MaxValue);
            var plural = vesselTypes.Count == 1 ? "" : "s";
            _logger.LogDebug($"{vesselTypes.Count} vessel type{plural} loaded via the service");

            // Create a list of select list items from the list of vessel types. Add an empty entry if there
            // is more than one. If not, the list will only contain that one vessel type which will be the default
            // selection
            if (vesselTypes.Count != 1)
            {
                list.Add(new SelectListItem() { Text = "", Value = "0" });
            }

            // Now add an entry for each vessel type
            foreach (var vesselType in vesselTypes)
            {
                list.Add(new SelectListItem() { Text = vesselType.Name, Value = vesselType.Id.ToString() });
            }

            return list;
        }
    }
}