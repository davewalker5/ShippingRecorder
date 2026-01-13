using ShippingRecorder.Entities.Db;
using ShippingRecorder.Mvc.Interfaces;
using ShippingRecorder.Mvc.Models;
using ShippingRecorder.Mvc.Wizard;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ShippingRecorder.Mvc.Controllers
{
    [Authorize]
    public class SightingDetailsController : ShippingRecorderControllerBase
    {
        private readonly AddSightingWizard _wizard;

        public SightingDetailsController(
            AddSightingWizard wizard,
            IPartialViewToStringRenderer renderer,
            ILogger<SightingDetailsController> logger) : base (renderer, logger)
        {
            _wizard = wizard;
        }

        /// <summary>
        /// Serve the sighting details entry page
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Index(int? id = null)
        {
            SightingDetailsViewModel model = await _wizard.GetSightingDetailsModelAsync(User.Identity.Name, id);
            return View(model);
        }

        /// <summary>
        /// Handle POST events to cache sighting details
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(SightingDetailsViewModel model)
        {
            IActionResult result;
            string newLocation = (model.NewLocation ?? "").Trim();
            bool haveLocation = (model.LocationId > 0) || !string.IsNullOrEmpty(newLocation);

            if (haveLocation && ModelState.IsValid)
            {
                _wizard.CacheSightingDetailsModel(model, User.Identity.Name);
                result = RedirectToAction("Index", "VesselDetails");
            }
            else
            {
                if (!haveLocation)
                {
                    model.LocationErrorMessage = "You must select a location or give a new location name";
                }

                List<Location> locations = await _wizard.GetLocationsAsync();
                model.SetLocations(locations);

                List<Voyage> voyages = await _wizard.GetVoyagesAsync();
                model.SetVoyages(voyages);

                result = View(model);
            }

            return result;
        }
    }
}
