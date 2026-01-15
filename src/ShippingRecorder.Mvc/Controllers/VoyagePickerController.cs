using HealthTracker.Mvc.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShippingRecorder.Client.Interfaces;
using ShippingRecorder.Mvc.Interfaces;
using ShippingRecorder.Mvc.Models;

namespace ShippingRecorder.Mvc.Controllers
{
    [Authorize]
    public class VoyagePickerController : ShippingRecorderControllerBase
    {
        private readonly IVoyageClient _voyageClient;
        private readonly IOperatorListGenerator _operatorListGenerator;

        public VoyagePickerController(
            IVoyageClient voyageClient,
            IOperatorListGenerator operatorListGenerator,
            IPartialViewToStringRenderer renderer,
            ILogger<OperatorsController> logger) : base (renderer, logger)
        {
            _voyageClient = voyageClient;
            _operatorListGenerator = operatorListGenerator;
        }

        /// <summary>
        /// Serve the voyage picker
        /// </summary>
        /// <param name="destinationId"></param>
        /// <param name="destinationName"></param>
        /// <returns></returns>
        [HttpGet]
        // [AjaxOnly]
        public async Task<IActionResult> Index(string destinationId, string destinationName)
        {
            var model = new VoyagePickerViewModel
            {
                Operators = await _operatorListGenerator.Create(),
                DestinationIdControl = destinationId,
                DestinationNameControl = destinationName
            };

            return await LoadModalContent("_VoyagePicker", model, "Voyage Selector");
        }

        /// <summary>
        /// Return a list of voyages for the operator with the specified ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [AjaxOnly]
        public async Task<IActionResult> Voyages(int id)
        {
            var voyages = await _voyageClient.ListAsync(id, 1, int.MaxValue);
            return PartialView("_VoyageList", voyages);
        }
    }
}