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
        private readonly IVoyageClient _voyageClient;
        private readonly IOperatorListGenerator _operatorListGenerator;
        private readonly IShippingRecorderApplicationSettings _settings;

        public VoyagesController(
            IVoyageClient voyageClient,
            IOperatorListGenerator operatorListGenerator,
            IShippingRecorderApplicationSettings settings,
            IPartialViewToStringRenderer renderer,
            ILogger<VoyagesController> logger) : base (renderer, logger)
        {
            _voyageClient = voyageClient;
            _operatorListGenerator = operatorListGenerator;
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

                List<Voyage> voyages = await _voyageClient.ListAsync(model.OperatorId.Value, page, _settings.SearchPageSize);
                model.SetVoyages(voyages, page, _settings.SearchPageSize);
            }
            else
            {
                LogModelState();
            }

            model.Operators = await _operatorListGenerator.Create();
            return View(model);
        }
    }
}
