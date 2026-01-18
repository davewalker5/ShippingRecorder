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
    public class SearchSightingsByVesselController : ShippingRecorderControllerBase
    {
        private readonly IVesselClient _vesselClient;
        private readonly ISightingClient _sightingClient;
        private readonly IShippingRecorderApplicationSettings _settings;


        public SearchSightingsByVesselController(
            IVesselClient vesselClient,
            ISightingClient sightingClient,
            IShippingRecorderApplicationSettings settings,
            IPartialViewToStringRenderer renderer,
            ILogger<SearchSightingsByVesselController> logger) : base (renderer, logger)
        {
            _vesselClient = vesselClient;
            _sightingClient = sightingClient;
            _settings = settings;
        }

        /// <summary>
        /// Serve the empty search sightings by aircraft page
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Index()
        {
            var model = new SightingSearchByVesselViewModel
            {
                PageNumber = 1
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
        public async Task<IActionResult> Index(SightingSearchByVesselViewModel model)
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

                List<Sighting> sightings = null;

                try
                {
                    // Retrieve the aircraft with the specified registration number
                    // then, if we have a valid aircraft, retrieve its sightings
                    Vessel vessel = await _vesselClient.GetAsync(model.Identifier);
                    if (vessel != null)
                    {
                        sightings = await _sightingClient.ListSightingsByVesselAsync(vessel.Id, page, _settings.SearchPageSize);
                    }
                }
                catch
                {
                }

                // Expose the sightings to the View
                model.SetSightings(sightings, page, _settings.SearchPageSize);
            }
            else
            {
                LogModelState();
            }

            return View(model);
        }
    }
}
