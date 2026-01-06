using System.Diagnostics;
using ShippingRecorder.Mvc.Interfaces;
using ShippingRecorder.Mvc.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using ShippingRecorder.Mvc.Wizard;

namespace ShippingRecorder.Mvc.Controllers
{
    [Authorize]
    public class HomeController : ShippingRecorderControllerBase
    {
        private AddSightingWizard _wizard;

        public HomeController(
            AddSightingWizard wizard,
            IPartialViewToStringRenderer renderer,
            ILogger<HomeController> logger) : base (renderer, logger)
        {
            _wizard = wizard;
        }

        /// <summary>
        /// Serve the models list page
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Index()
        {
            // If we hit the home page via this action method, reset the add sighting wizard
            // as we're starting from scratch with a new entry
            _wizard.Reset(User.Identity.Name);
            return RedirectToAction("Index", "SightingDetails");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            // Construct the error view model
            var exceptionFeature = HttpContext.Features.Get<IExceptionHandlerPathFeature>();
            var model = new ErrorViewModel
            {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier,
                ErrorMessage = exceptionFeature != null ? exceptionFeature.Error.Message : ""
            };

            return View(model);
        }
    }
}
