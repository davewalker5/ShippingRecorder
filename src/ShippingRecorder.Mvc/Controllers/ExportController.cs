using ShippingRecorder.Client.Interfaces;
using ShippingRecorder.Mvc.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShippingRecorder.Mvc.Interfaces;
using ShippingRecorder.Mvc.Enumerations;

namespace ShippingRecorder.Mvc.Controllers
{
    [Authorize]
    public class ExportController : DataExchangeControllerBase
    {
        public ExportController(
            ICountryClient countryClient,
            ILocationClient locationClient,
            IOperatorClient operatorClient,
            IPortClient portClient,
            ISightingClient sightingClient,
            IVesselClient vesselClient,
            IVesselTypeClient vesselTypeClient,
            IPartialViewToStringRenderer renderer,
            ILogger<ExportController> logger) : base(
                countryClient,
                locationClient,
                operatorClient,
                portClient,
                sightingClient,
                vesselClient,
                vesselTypeClient,
                renderer,
                logger
            )
        {
        }

        /// <summary>
        /// Serve the generic data export page
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Index()
            => View(new ExportViewModel());

        /// <summary>
        /// Handle POST events on the generic data export page
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(ExportViewModel model)
        {
            // If the model's nominally valid, perform some additional checks
            if (ModelState.IsValid)
            {
                // Check the data exchange type isn't the default selection, "None"
                _logger.LogDebug($"Import data type = {model.DataExchangeType}");
                if (model.DataExchangeType == DataExchangeType.None)
                {
                    ModelState.AddModelError("DataExchangeType", "You must select an import data type");
                }
            }


            // If the model's still valid, proceed with the export
            if (ModelState.IsValid)
            {
                // Request the import
                _logger.LogDebug($"Requesting import of {model.DataExchangeTypeName} data from {model.FileName}");
                await ExportClient(model.DataExchangeType).ExportAsync(model.FileName);

                // Reset the model and set a confirmation message
                ModelState.Clear();
                model.Message = $"Export of {model.DataExchangeTypeName} data to {model.FileName} has been requested";
                model.DataExchangeType = DataExchangeType.None;
                model.FileName = null;
            }
            else
            {
                LogModelState();
            }

            return View(model);
        }
    }
}