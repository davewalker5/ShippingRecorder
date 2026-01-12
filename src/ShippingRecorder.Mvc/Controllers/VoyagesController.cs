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
    public class VoyagesController : ShippingRecorderControllerBase
    {
        private readonly IVoyageClient _client;
        private readonly IOperatorListGenerator _operatorListGenerator;
        private readonly IVesselListGenerator _vesselListGenerator;
        private readonly IShippingRecorderApplicationSettings _settings;

        public VoyagesController(
            IVoyageClient voyageClient,
            IOperatorListGenerator operatorListGenerator,
            IVesselListGenerator vesselListGenerator,
            IShippingRecorderApplicationSettings settings,
            IPartialViewToStringRenderer renderer,
            ILogger<VoyagesController> logger) : base (renderer, logger)
        {
            _client = voyageClient;
            _operatorListGenerator = operatorListGenerator;
            _vesselListGenerator = vesselListGenerator;
            _settings = settings;
        }

        /// <summary>
        /// Serve the empty search sightings by date range
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var model = new VoyageSearchViewModel
            {
                PageNumber = 1,
                Operators = await _operatorListGenerator.Create()
            };
            return View(model);
        }

        /// <summary>
        /// Respond to a POST event triggering the search
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(VoyageSearchViewModel model)
        {
            if (ModelState.IsValid)
            {
                int page = model.PageNumber;
                switch (model.Action)
                {
                    case ControllerActions.ActionAdd:
                        return RedirectToAction("Add");
                    case ControllerActions.ActionPreviousPage:
                        page -= 1;
                        break;
                    case ControllerActions.ActionNextPage:
                        page += 1;
                        break;
                    case ControllerActions.ActionSearch:
                        page = 1;
                        break;
                    default:
                        break;
                }

                // Need to clear model state here or the page number that was posted
                // is returned and page navigation doesn't work correctly. So, capture
                // and amend the page number, above, then apply it, below
                ModelState.Clear();

                List<Voyage> voyages = await _client.ListAsync(model.OperatorId.Value, page, _settings.SearchPageSize);
                model.SetVoyages(voyages, page, _settings.SearchPageSize);
            }
            else
            {
                LogModelState();
            }

            model.Operators = await _operatorListGenerator.Create();
            return View(model);
        }

        /// <summary>
        /// Serve the page to add a new voyage
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Add()
        {
            return View(new AddVoyageViewModel()
            {
                Operators = await _operatorListGenerator.Create(),
                Vessels = await _vesselListGenerator.Create()
            });
        }

        /// <summary>
        /// Handle POST events to save new voyages
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(AddVoyageViewModel model)
        {
            if (ModelState.IsValid)
            {
                _logger.LogDebug($"Adding voyage: Operator ID = {model.OperatorId}, Vessel Id = {model.VesselId}, Number = {model.Number}");
                Voyage voyage = await _client.AddAsync(model.OperatorId, model.VesselId, model.Number);
                ModelState.Clear();
                model.Clear();
                // TODO : Redirect to the voyage details page so events can be added
                model.Message = $"Voyage '{voyage.Number}' added successfully";
            }
            else
            {
                LogModelState();
            }

            // Load ancillary data then render the view
            model.Operators = await _operatorListGenerator.Create();
            model.Vessels = await _vesselListGenerator.Create();

            return View(model);
        }
    }
}
