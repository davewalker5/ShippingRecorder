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
        /// Serve the voyage/event editing page
        /// </summary>
        /// <param name="voyageId"></param>
        /// <param name="eventId"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Index(long voyageId, long eventId = 0)
        {
            _logger.LogDebug($"Editing event {eventId} for voyage {voyageId}");

            // Load the voyage
            var voyage = await _voyageClient.GetAsync(voyageId);
            _logger.LogDebug($"Retrieved voyage : {voyage}");

            // Create the model
            var model = new VoyageEventViewModel
            {
                VoyageId = voyageId,
                VoyageNumber = voyage.Number
            };

            // If we have an existing event ID, set the event properties on the model from that event
            var evt = eventId > 0 ? voyage.Events.FirstOrDefault(x => x.Id == eventId) : null;
            if (evt != null)
            {
                _logger.LogDebug($"Setting properties for event: {evt}");
                model.Date = evt.Date;
                model.Port = evt.Port.Code;
                model.EventType = evt.EventType;
            }

            return View(model);
        }
    }
}