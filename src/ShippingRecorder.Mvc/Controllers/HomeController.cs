using System.Diagnostics;
using ShippingRecorder.Mvc.Interfaces;
using ShippingRecorder.Mvc.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace ShippingRecorder.Mvc.Controllers
{
    [Authorize]
    public class HomeController : ShippingRecorderControllerBase
    {
        public HomeController(
            IPartialViewToStringRenderer renderer,
            ILogger<HomeController> logger) : base (renderer, logger)
        {
        }

        /// <summary>
        /// Serve the models list page
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Index()
        {
            return View();
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
