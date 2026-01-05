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
    public class SearchSightingsByLocationController : ShippingRecorderControllerBase
    {
        private readonly ILocationListGenerator _locationListGenerator;
        private readonly ISightingClient _sightingClient;
        private readonly IShippingRecorderApplicationSettings _settings;

        public SearchSightingsByLocationController(
            ILocationListGenerator locationListGenerator,
            ISightingClient sightingClient,
            IShippingRecorderApplicationSettings settings,
            IPartialViewToStringRenderer renderer,
            ILogger<SearchSightingsByLocationController> logger) : base (renderer, logger)
        {
            _locationListGenerator = locationListGenerator;
            _sightingClient = sightingClient;
            _settings = settings;
        }

        /// <summary>
        /// Serve the empty search sightings by airline page
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            SightingSearchByLocationViewModel model = new SightingSearchByLocationViewModel
            {
                PageNumber = 1,
                Locations = await _locationListGenerator.Create()
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
        public async Task<IActionResult> Index(SightingSearchByLocationViewModel model)
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
                // is returned and page navigation doesn't work correctly
                ModelState.Clear();

                List<Sighting> sightings = await _sightingClient.ListSightingsByLocationAsync(model.LocationId ?? 0, page, _settings.SearchPageSize);
                model.SetSightings(sightings, page, _settings.SearchPageSize);
            }
            else
            {
                LogModelState();
            }

            model.Locations = await _locationListGenerator.Create();
            return View(model);
        }
    }
}
