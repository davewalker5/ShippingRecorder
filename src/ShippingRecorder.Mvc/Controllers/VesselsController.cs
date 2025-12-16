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
    public class VesselsController : ShippingRecorderControllerBase
    {
        private readonly IVesselClient _client;
        private readonly IShippingRecorderApplicationSettings _settings;

        public VesselsController(
            IVesselClient client,
            IShippingRecorderApplicationSettings settings,
            IPartialViewToStringRenderer renderer,
            ILogger<VesselsController> logger) : base (renderer, logger)
        {
            _client = client;
            _settings = settings;
        }

        /// <summary>
        /// Serve the vessel list page
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            // Get the list of current vessels
            List<Vessel> vessels = await _client.ListAsync(1, _settings.SearchPageSize) ?? [];
            var plural = vessels.Count == 1 ? "" : "s";
            _logger.LogDebug($"{vessels.Count} vessel{plural} loaded via the service");

            // Construct the view model and serve the page
            var model = new VesselListViewModel();
            model.SetVessels(vessels, 1, _settings.SearchPageSize);
            return View(model);
        }

        /// <summary>
        /// Handle POST events for page navigation
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(VesselListViewModel model)
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
                var vessels = await _client.ListAsync(page, _settings.SearchPageSize);
                model.SetVessels(vessels, page, _settings.SearchPageSize);
            }
            else
            {
                LogModelState();
            }

            return View(model);
        }

        /// <summary>
        /// Serve the page to add a new vessel
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Add()
        {
            return View(new AddVesselViewModel());
        }

        /// <summary>
        /// Handle POST events to save new vessels
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(AddVesselViewModel model)
        {
            if (ModelState.IsValid)
            {
                _logger.LogDebug($"Adding vessel: IMO = {model.IMO}, Built = {model.Built}, Draught = {model.Draught}, Length = {model.Length}, Beam = {model.Beam}");
                Vessel vessel = await _client.AddAsync(model.IMO, model.Built, model.Draught, model.Length, model.Beam);
                ModelState.Clear();
                model.Clear();
                model.Message = $"Vessel '{vessel.IMO}' added successfully";
            }
            else
            {
                LogModelState();
            }

            return View(model);
        }

        /// <summary>
        /// Serve the vessel editing page
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            Vessel model = await _client.GetAsync(id);
            return View(model);
        }

        /// <summary>
        /// Handle POST events to save updated vessels
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Vessel model)
        {
            IActionResult result;

            if (ModelState.IsValid)
            {
                _logger.LogDebug($"Updating vessel: ID = {model.Id}, IMO = {model.IMO}, Built = {model.Built}, Draught = {model.Draught}, Length = {model.Length}, Beam = {model.Beam}");
                await _client.UpdateAsync(model.Id, model.IMO, model.Built, model.Draught, model.Length, model.Beam);
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
