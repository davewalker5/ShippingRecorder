using ShippingRecorder.Mvc.Entities;
using ShippingRecorder.Mvc.Interfaces;
using ShippingRecorder.Mvc.Models;
using ShippingRecorder.Mvc.Wizard;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ShippingRecorder.Mvc.Controllers
{
    [Authorize]
    public class ConfirmDetailsController : ShippingRecorderControllerBase
    {
        private AddSightingWizard _wizard;

        public ConfirmDetailsController(
            AddSightingWizard wizard,
            IPartialViewToStringRenderer renderer,
            ILogger<ConfirmDetailsController> logger) : base (renderer, logger)
        {
            _wizard = wizard;
        }

        /// <summary>
        /// Serve the confirm details page
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            ConfirmDetailsViewModel model = await _wizard.GetConfirmDetailsModelAsync(User.Identity.Name);
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
        public async Task<IActionResult> Index(ConfirmDetailsViewModel viewModel)
        {
            IActionResult result = null;

            switch (viewModel.Action)
            {
                case ControllerActions.ActionNextPage:
                    await _wizard.CreateSighting(User.Identity.Name);
                    result = RedirectToAction("Index", "SightingDetails");
                    break;
                case ControllerActions.ActionPreviousPage:
                    result = RedirectToAction("Index", "VesselDetails");
                    break;
                default:
                    break;
            }

            return result;
        }
    }
}
