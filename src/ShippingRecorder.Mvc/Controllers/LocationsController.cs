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
    public class LocationsController : ShippingRecorderControllerBase
    {
        private readonly ILocationClient _client;
        private readonly IShippingRecorderApplicationSettings _settings;

        public LocationsController(
            ILocationClient client,
            IShippingRecorderApplicationSettings settings,
            IPartialViewToStringRenderer renderer,
            ILogger<LocationsController> logger) : base (renderer, logger)
        {
            _client = client;
            _settings = settings;
        }

        /// <summary>
        /// Serve the locations list page
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            // Get the list of current locations
            List<Location> locations = await _client.ListAsync(1, _settings.SearchPageSize);
            var plural = locations.Count == 1 ? "" : "s";
            _logger.LogDebug($"{locations.Count} location{plural} loaded via the service");

            // Construct the view model and serve the page
            var model = new LocationListViewModel();
            model.SetLocations(locations, 1, _settings.SearchPageSize);
            return View(model);
        }

        /// <summary>
        /// Handle POST events for page navigation
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(LocationListViewModel model)
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

                // Retrieve the matching airport records
                var locations = await _client.ListAsync(page, _settings.SearchPageSize);
                model.SetLocations(locations, page, _settings.SearchPageSize);
            }
            else
            {
                LogModelState();
            }

            return View(model);
        }

        /// <summary>
        /// Serve the page to add a new location
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Add()
        {
            return View(new AddLocationViewModel());
        }

        /// <summary>
        /// Handle POST events to save new locations
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(AddLocationViewModel model)
        {
            if (ModelState.IsValid)
            {
                _logger.LogDebug($"Adding location: Name = {model.Name}");
                Location location = await _client.AddAsync(model.Name);
                ModelState.Clear();
                model.Clear();
                model.Message = $"Location '{location.Name}' added successfully";
            }
            else
            {
                LogModelState();
            }

            return View(model);
        }

        /// <summary>
        /// Serve the location editing page
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            Location model = await _client.GetAsync(id);
            return View(model);
        }

        /// <summary>
        /// Handle POST events to save updated locations
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Location model)
        {
            IActionResult result;

            if (ModelState.IsValid)
            {
                _logger.LogDebug($"Updating location: ID = {model.Id}, Name = {model.Name}");
                await _client.UpdateAsync(model.Id, model.Name);
                result = RedirectToAction("Index");
            }
            else
            {
                LogModelState();
                result = View(model);
            }

            return result;
        }
    }
}
