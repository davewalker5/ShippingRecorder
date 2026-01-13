using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShippingRecorder.Client.Interfaces;
using ShippingRecorder.Entities.Interfaces;
using ShippingRecorder.Mvc.Interfaces;
using ShippingRecorder.Mvc.Models;

namespace ShippingRecorder.Mvc.Controllers
{
    [Authorize]
    public class VoyageEventsController : ShippingRecorderControllerBase
    {
        private readonly IVoyageClient _voyageClient;
        private readonly IVoyageEventClient _voyageEventClient;
        private readonly IShippingRecorderApplicationSettings _settings;

        public VoyageEventsController(
            IVoyageClient voyageClient,
            IVoyageEventClient voyageEventClient,
            IShippingRecorderApplicationSettings settings,
            IPartialViewToStringRenderer renderer,
            ILogger<VoyagesController> logger) : base (renderer, logger)
        {
            _voyageClient = voyageClient;
            _voyageEventClient = voyageEventClient;
            _settings = settings;
        }

        /// <summary>
        /// Serve the voyage/event addition/editing page
        /// </summary>
        /// <param name="voyageId"></param>
        /// <param name="eventId"></param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Index(long voyageId, long eventId = 0)
            => eventId > 0 ?
                RedirectToAction("Edit", new { voyageId = voyageId, eventId = eventId }) :
                RedirectToAction("Add", new { voyageId = voyageId });

        /// <summary>
        /// Serve the voyage/event addition page
        /// </summary>
        /// <param name="voyageId"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Add(long voyageId)
        {
            _logger.LogDebug($"Adding new event to voyage {voyageId}");
            var model = await BuildModel(voyageId, 0);
            return View(model);
        }

        /// <summary>
        /// Serve the voyage/event editing page
        /// </summary>
        /// <param name="voyageId"></param>
        /// <param name="eventId"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Edit(long voyageId, long eventId = 0)
        {
            _logger.LogDebug($"Editing event {eventId} for voyage {voyageId}");
            var model = await BuildModel(voyageId, eventId);
            return View(model);
        }

        /// <summary>
        /// Build a voyage/event model
        /// </summary>
        /// <param name="voyageId"></param>
        /// <param name="eventId"></param>
        /// <returns></returns>
        private async Task<VoyageEventViewModel> BuildModel(long voyageId, long eventId)
        {
            // Load the voyage
            var voyage = await _voyageClient.GetAsync(voyageId);
            _logger.LogDebug($"Retrieved voyage : {voyage}");

            // Create the model
            var model = new VoyageEventViewModel
            {
                VoyageId = voyageId,
                VoyageNumber = voyage.Number
            };

            // Set the event properties on the model from the specified event
            var evt = eventId > 0 ? voyage.Events.FirstOrDefault(x => x.Id == eventId) : null;
            if (evt != null)
            {
                _logger.LogDebug($"Setting properties for event: {evt}");
                model.Date = evt.Date;
                model.Port = evt.Port.Code;
                model.EventType = evt.EventType;
            }

            return model;
        }
    }
}