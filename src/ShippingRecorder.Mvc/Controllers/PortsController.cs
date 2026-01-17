using ShippingRecorder.Client.Interfaces;
using ShippingRecorder.Entities.Db;
using ShippingRecorder.Mvc.Entities;
using ShippingRecorder.Mvc.Interfaces;
using ShippingRecorder.Mvc.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShippingRecorder.Entities.Interfaces;

namespace ShippingRecorder.Mvc.Controllers
{
    [Authorize]
    public class PortsController : ShippingRecorderControllerBase
    {
        private readonly IPortClient _portClient;
        private readonly ICountryClient _countryClient;
        private readonly ICountryListGenerator _countryListGenerator;
        private readonly IShippingRecorderApplicationSettings _settings;

        public PortsController(
            IPortClient portClient,
            ICountryClient countryClient,
            ICountryListGenerator countryListGenerator,
            IShippingRecorderApplicationSettings settings,
            IPartialViewToStringRenderer renderer,
            ILogger<PortsController> logger) : base (renderer, logger)
        {
            _portClient = portClient;
            _countryClient = countryClient;
            _countryListGenerator = countryListGenerator;
            _settings = settings;
        }

        /// <summary>
        /// Serve the ports list page
        /// </summary>
        /// <param name="countryId"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Index(int countryId = 0)
        {
            // Get the list of current ports for the specified country, or all ports if the ID is 0
            List<Port> ports = await _portClient.ListAsync(countryId, 1, _settings.SearchPageSize) ?? [];
            var plural = ports.Count == 1 ? "" : "s";
            _logger.LogDebug($"{ports.Count} port{plural} loaded via the service");

            // Construct the view model and serve the page
            var model = new PortListViewModel
            {
                CountryId = countryId,
                Countries = await _countryListGenerator.Create()
            };
            model.SetPorts(ports, 1, _settings.SearchPageSize);
            return View(model);
        }

        /// <summary>
        /// Handle POST events for page navigation
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(PortListViewModel model)
        {
            if (ModelState.IsValid)
            {
                int page = model.PageNumber;
                switch (model.Action)
                {
                    case ControllerActions.ActionPreviousPage:
                        page -= 1;
                        break;
                    case ControllerActions.ActionNextPage:
                        page += 1;
                        break;
                    default:
                        break;
                }

                // Need to clear model state here or the page number that was posted
                // is returned and page navigation doesn't work correctly. So, capture
                // and amend the page number, above, then apply it, below
                ModelState.Clear();

                // Retrieve the matching port records
                var ports = await _portClient.ListAsync(model.CountryId, page, _settings.SearchPageSize);
                model.SetPorts(ports, page, _settings.SearchPageSize);
            }
            else
            {
                LogModelState();
            }

            // Reload the countries list
            model.Countries = await _countryListGenerator.Create();
            return View(model);
        }

        /// <summary>
        /// Serve the page to add a new port
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Add()
        {
            return View(new AddPortViewModel
            {
                Countries = await _countryListGenerator.Create()
            });
        }

        /// <summary>
        /// Handle POST events to save new ports
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(AddPortViewModel model)
        {
            // If the model is nominally valid, make sure the UN/LOCODE matches the country
            if (ModelState.IsValid)
            {
                var country = await _countryClient.GetAsync(model.CountryId);
                var unlocode = model.Code.ToUpper();
                if (country.Code != unlocode[..2])
                {
                    ModelState.AddModelError("Code", "The beginning of the UN/LOCODE must match the country code");
                }
            }

            if (ModelState.IsValid)
            {
                _logger.LogDebug($"Adding port: CountryId = {model.CountryId}, UN/LOCODE = {model.Code}, Name = {model.Name}");
                Port port = await _portClient.AddAsync(model.CountryId, model.Code, model.Name);
                ModelState.Clear();
                model.Clear();
                model.Message = $"Port '{port.Code}' added successfully";
            }
            else
            {
                LogModelState();
            }

            // Reload the countries list
            model.Countries = await _countryListGenerator.Create();
            return View(model);
        }

        /// <summary>
        /// Serve the port editing page
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Edit(long id)
        {
            var port = await _portClient.GetAsync(id);
            var model = new EditPortViewModel
            {
                Countries = await _countryListGenerator.Create(),
                CountryId = port.CountryId,
                Code = port.Code,
                Name = port.Name
            };
            return View(model);
        }

        /// <summary>
        /// Handle POST events to save updated ports
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditPortViewModel model)
        {
            IActionResult result;

            // If the model is nominally valid, make sure the UN/LOCODE matches the country
            if (ModelState.IsValid)
            {
                var country = await _countryClient.GetAsync(model.CountryId);
                var unlocode = model.Code.ToUpper();
                if (country.Code != unlocode[..2])
                {
                    ModelState.AddModelError("Code", "The beginning of the UN/LOCODE must match the country code");
                }
            }

            if (ModelState.IsValid)
            {
                _logger.LogDebug($"Updating port: ID = {model.Id}, Country ID = {model.CountryId}, Code = {model.Code}, Name = {model.Name}");
                await _portClient.UpdateAsync(model.Id, model.CountryId, model.Code, model.Name);
                result = RedirectToAction("Index", new { countryId = model.CountryId });
            }
            else
            {
                LogModelState();
                result = View(model);
            }

            return result;
        }

        /// <summary>
        /// Handle POST events to delete an existing voyage
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete([FromForm] long id)
        {
            // Retrieve the port
            var port = await _portClient.GetAsync(id);
            _logger.LogDebug($"Retrieved port {port}");

            // Delete the port
            _logger.LogDebug($"Deleting port: ID = {id}");
            await _portClient.DeleteAsync(id);

            return RedirectToAction("Index", new { countryId = port.CountryId });
        }
    }
}
