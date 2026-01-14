using HealthTracker.Mvc.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShippingRecorder.Client.Interfaces;
using ShippingRecorder.Mvc.Interfaces;
using ShippingRecorder.Mvc.Models;

namespace ShippingRecorder.Mvc.Controllers
{
    [Authorize]
    public class PortPickerController : ShippingRecorderControllerBase
    {
        private readonly IPortClient _portClient;
        private readonly ICountryListGenerator _countryListGenerator;

        public PortPickerController(
            IPortClient portClient,
            ICountryListGenerator countryListGenerator,
            IPartialViewToStringRenderer renderer,
            ILogger<OperatorsController> logger) : base (renderer, logger)
        {
            _portClient = portClient;
            _countryListGenerator = countryListGenerator;
        }


        /// <summary>
        /// Serve the port picker
        /// </summary>
        /// <param name="destinationControl"></param>
        /// <returns></returns>
        [HttpGet]
        [AjaxOnly]
        public async Task<IActionResult> Index(string destinationControl)
        {
            var model = new PortPickerViewModel
            {
                Countries = await _countryListGenerator.Create(),
                DestinationControl = destinationControl
            };

            return await LoadModalContent("_PortPicker", model, "Port Selector");
        }

        /// <summary>
        /// Return a list of ports for the country with the specified ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [AjaxOnly]
        public async Task<IActionResult> Ports(int id, string searchTerm)
        {
            var ports = await _portClient.ListAsync(id, 1, int.MaxValue);
            var filtered = string.IsNullOrEmpty(searchTerm) ? ports : ports.Where(x => x.Name.Contains(searchTerm, StringComparison.OrdinalIgnoreCase));
            return PartialView("_PortList", filtered);
        }
    }
}