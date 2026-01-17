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
    public class VesselTypesController : ShippingRecorderControllerBase
    {
        private readonly IVesselTypeClient _client;
        private readonly IShippingRecorderApplicationSettings _settings;

        public VesselTypesController(
            IVesselTypeClient client,
            IShippingRecorderApplicationSettings settings,
            IPartialViewToStringRenderer renderer,
            ILogger<VesselTypesController> logger) : base (renderer, logger)
        {
            _client = client;
            _settings = settings;
        }

        /// <summary>
        /// Serve the vessel types list page
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            // Get the list of current vessel types
            List<VesselType> vesselTypes = await _client.ListAsync(1, _settings.SearchPageSize) ?? [];
            var plural = vesselTypes.Count == 1 ? "" : "s";
            _logger.LogDebug($"{vesselTypes.Count} vessel type{plural} loaded via the service");

            // Construct the view model and serve the page
            var model = new VesselTypeListViewModel();
            model.SetVesselTypes(vesselTypes, 1, _settings.SearchPageSize);
            return View(model);
        }

        /// <summary>
        /// Handle POST events for page navigation
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(VesselTypeListViewModel model)
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

                // Retrieve the matching vessel type records
                var vesselTypes = await _client.ListAsync(page, _settings.SearchPageSize);
                model.SetVesselTypes(vesselTypes, page, _settings.SearchPageSize);
            }
            else
            {
                LogModelState();
            }

            return View(model);
        }

        /// <summary>
        /// Serve the page to add a new vessel type
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Add()
        {
            return View(new AddVesselTypeViewModel());
        }

        /// <summary>
        /// Handle POST events to save new vessel types
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(AddVesselTypeViewModel model)
        {
            if (ModelState.IsValid)
            {
                _logger.LogDebug($"Adding vessel type: Name = {model.Name}");
                VesselType vesselType = await _client.AddAsync(model.Name);
                ModelState.Clear();
                model.Clear();
                model.Message = $"VesselType '{vesselType.Name}' added successfully";
            }
            else
            {
                LogModelState();
            }

            return View(model);
        }

        /// <summary>
        /// Serve the vessel type editing page
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            VesselType model = await _client.GetAsync(id);
            return View(model);
        }

        /// <summary>
        /// Handle POST events to save updated vessel types
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(VesselType model)
        {
            IActionResult result;

            if (ModelState.IsValid)
            {
                _logger.LogDebug($"Updating vessel type: ID = {model.Id}, Name = {model.Name}");
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
