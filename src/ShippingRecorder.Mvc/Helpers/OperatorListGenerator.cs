using ShippingRecorder.Client.Interfaces;
using ShippingRecorder.Mvc.Interfaces;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace HealthTracker.Mvc.Helpers
{
    public class OperatorListGenerator : IOperatorListGenerator
    {
        private readonly IOperatorClient _client;
        private readonly ILogger<OperatorListGenerator> _logger;

        public OperatorListGenerator(IOperatorClient client, ILogger<OperatorListGenerator> logger)
        {
            _client = client;
            _logger = logger;
        }

        /// <summary>
        /// Generate a list of select list items for operators
        /// </summary>
        /// <returns></returns>
        public async Task<IList<SelectListItem>> Create()
        {
            var list = new List<SelectListItem>();

            // Load the list of operators
            var operators = await _client.ListAsync(1, int.MaxValue);
            var plural = operators.Count == 1 ? "" : "s";
            _logger.LogDebug($"{operators.Count} operator{plural} loaded via the service");

            // Create a list of select list items from the list of operators. Add an empty entry if there
            // is more than one. If not, the list will only contain that one operator which will be the default
            // selection
            if (operators.Count != 1)
            {
                list.Add(new SelectListItem() { Text = "", Value = "0" });
            }

            // Now add an entry for each operator
            foreach (var op in operators)
            {
                list.Add(new SelectListItem() { Text = op.Name, Value = op.Id.ToString() });
            }

            return list;
        }
    }
}