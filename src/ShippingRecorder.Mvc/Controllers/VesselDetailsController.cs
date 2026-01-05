using ShippingRecorder.Mvc.Entities;
using ShippingRecorder.Mvc.Interfaces;
using ShippingRecorder.Mvc.Models;
using ShippingRecorder.Mvc.Wizard;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ShippingRecorder.Mvc.Controllers
{
    [Authorize]
    public class VesselDetailsController : ShippingRecorderControllerBase
    {
        private readonly AddSightingWizard _wizard;
        private readonly ICountryListGenerator _countryListGenerator;
        private readonly IOperatorListGenerator _operatorListGenerator;
        private readonly IVesselTypeListGenerator _vesselTypesListGenerator;


        public VesselDetailsController(
            AddSightingWizard wizard,
            ICountryListGenerator countryListGenertor,
            IOperatorListGenerator operatorListGenerator,
            IVesselTypeListGenerator vesselTypesListGenertor,
            IPartialViewToStringRenderer renderer,
            ILogger<VesselDetailsController> logger) : base (renderer, logger)
        {
            _wizard = wizard;
            _countryListGenerator = countryListGenertor;
            _operatorListGenerator = operatorListGenerator;
            _vesselTypesListGenerator = vesselTypesListGenertor;
        }

        /// <summary>
        /// Serve the aircraft details entry page
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            VesselDetailsViewModel model = await _wizard.GetVesselDetailsModelAsync(User.Identity.Name);
            return View(model);
        }

        /// <summary>
        /// Handle POST events to cache aircraft details or move back to the flight
        /// details page
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(VesselDetailsViewModel model)
        {
            IActionResult result = null;

            // If the model isn't editable, ignore view state errors (mandatory fields won't be posted back
            // because the form controls are marked as disabled)
            if (!model.Editable)
            {
                foreach (var entry in ModelState.Values)
                {
                    entry.Errors.Clear();
                }
            }

            if (ModelState.IsValid && (model.Action == ControllerActions.ActionNextPage))
            {
                _wizard.CacheVesselDetailsModel(model, User.Identity.Name);
                // TODO: Redirect to the next step
                // result = RedirectToAction("Index", "ConfirmDetails");
                result = View(model);
            }
            else if (model.Action == ControllerActions.ActionPreviousPage)
            {
                _wizard.ClearCachedVesselDetailsModel(User.Identity.Name);
                result = RedirectToAction("Index", "SightingDetails");
            }
            else
            {
                LogModelState();

                // Load vessel types, countries and operators
                model.VesselTypes = await _vesselTypesListGenerator.Create();
                model.Countries = await _countryListGenerator.Create();
                model.Operators = await _operatorListGenerator.Create();

                result = View(model);
            }

            return result;
        }
    }
}
