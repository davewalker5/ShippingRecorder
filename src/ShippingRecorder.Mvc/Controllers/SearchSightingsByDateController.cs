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
    public class SearchSightingsByDateController : ShippingRecorderControllerBase
    {
        private readonly ISightingClient _sightingClient;
        private readonly IShippingRecorderApplicationSettings _settings;

        public SearchSightingsByDateController(
            ISightingClient sightingClient,
            IShippingRecorderApplicationSettings settings,
            IPartialViewToStringRenderer renderer,
            ILogger<SearchSightingsByDateController> logger) : base (renderer, logger)
        {
            _sightingClient = sightingClient;
            _settings = settings;
        }

        /// <summary>
        /// Serve the empty search sightings by date range
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Index()
        {
            var model = new SightingSearchByDateViewModel
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
        public async Task<IActionResult> Index(SightingSearchByDateViewModel model)
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

                DateTime start = model.From ?? DateTime.MinValue;
                DateTime end = model.To ?? DateTime.MaxValue;

                List<Sighting> sightings = await _sightingClient.ListSightingsByDateAsync(start, end, page, _settings.SearchPageSize);
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
