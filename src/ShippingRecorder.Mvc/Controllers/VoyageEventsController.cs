using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShippingRecorder.Client.Interfaces;
using ShippingRecorder.Entities.Db;
using ShippingRecorder.Entities.Interfaces;
using ShippingRecorder.Mvc.Interfaces;
using ShippingRecorder.Mvc.Models;

namespace ShippingRecorder.Mvc.Controllers
{
    [Authorize]
    public class VoyageEventsController : ShippingRecorderControllerBase
    {
        private readonly IVoyageClient _client;
        private readonly IOperatorListGenerator _operatorListGenerator;
        private readonly IVesselListGenerator _vesselListGenerator;
        private readonly IShippingRecorderApplicationSettings _settings;

        public VoyageEventsController(
            IVoyageClient client,
            IOperatorListGenerator operatorListGenerator,
            IVesselListGenerator vesselListGenerator,
            IShippingRecorderApplicationSettings settings,
            IPartialViewToStringRenderer renderer,
            ILogger<VoyagesController> logger) : base (renderer, logger)
        {
            _client = client;
            _operatorListGenerator = operatorListGenerator;
            _vesselListGenerator = vesselListGenerator;
            _settings = settings;
        }

        /// <summary>
        /// Serve the voyage builder page
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Index(int id)
        {
            var model = new VoyageEventsViewModel
            {
                Voyage = await _client.GetAsync(id),
                Operators = await _operatorListGenerator.Create(),
                Vessels = await _vesselListGenerator.Create()
            };
            return View(model);
        }
    }
}