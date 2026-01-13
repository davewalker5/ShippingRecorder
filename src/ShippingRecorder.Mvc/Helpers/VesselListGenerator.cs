using ShippingRecorder.Client.Interfaces;
using ShippingRecorder.Mvc.Interfaces;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace HealthTracker.Mvc.Helpers
{
    public class VesselListGenerator : IVesselListGenerator
    {
        private readonly IVesselClient _client;
        private readonly ILogger<VesselListGenerator> _logger;

        public VesselListGenerator(IVesselClient client, ILogger<VesselListGenerator> logger)
        {
            _client = client;
            _logger = logger;
        }

        /// <summary>
        /// Generate a list of select list items for vessels
        /// </summary>
        /// <returns></returns>
        public async Task<IList<SelectListItem>> Create()
        {
            var list = new List<SelectListItem>();

            // Load the list of vessels
            var vessels = await _client.ListAsync(1, int.MaxValue);
            var count = vessels?.Count ?? 0;
            var plural = count == 1 ? "" : "s";
            _logger.LogDebug($"{count} vessel{plural} loaded via the service");

            // Create a list of select list items from the list of vessels. Add an empty entry if there
            // is more than one. If not, the list will only contain that one vessel which will be the default
            // selection
            if (count != 1)
            {
                list.Add(new SelectListItem() { Text = "", Value = "0" });
            }

            // Now add an entry for each vessel
            if (count > 0)
            {
                foreach (var vessel in vessels)
                {
                    // Construct the list entry from the IMO and name, if available
                    var text = vessel.ActiveRegistrationHistory != null ?
                        $"{vessel.IMO} - {vessel.ActiveRegistrationHistory?.Name}" :
                        vessel.IMO;

                    list.Add(new SelectListItem() { Text = text, Value = vessel.Id.ToString() });
                }
            }

            return list;
        }
    }
}