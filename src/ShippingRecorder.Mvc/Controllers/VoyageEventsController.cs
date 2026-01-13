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
        private readonly IPortClient _portClient;
        private readonly IVoyageEventClient _voyageEventClient;
        private readonly IShippingRecorderApplicationSettings _settings;

        public VoyageEventsController(
            IVoyageClient voyageClient,
            IPortClient portClient,
            IVoyageEventClient voyageEventClient,
            IShippingRecorderApplicationSettings settings,
            IPartialViewToStringRenderer renderer,
            ILogger<VoyagesController> logger) : base (renderer, logger)
        {
            _voyageClient = voyageClient;
            _portClient = portClient;
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
            var model = await BuildModel<AddVoyageEventViewModel>(voyageId, 0);
            return View(model);
        }

        /// <summary>
        /// Handle POST events to save new voyage events
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(AddVoyageEventViewModel model)
        {
            if (ModelState.IsValid)
            {
                _logger.LogDebug($"Adding voyage event: " + 
                    $"Voyage ID = {model.VoyageId}, " + 
                    $"Date = {model.Date}, " + 
                    $"Port = {model.Port}, " + 
                    $"EventType = {model.EventType}");

                Port port = await _portClient.GetAsync(model.Port);
                _ = await _voyageEventClient.AddAsync(model.VoyageId, model.EventType, port.Id, model.Date);
                return RedirectToAction("Index", "VoyageBuilder", new { id = model.VoyageId });
            }
            else
            {
                LogModelState();
            }

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
            var model = await BuildModel<EditVoyageEventViewModel>(voyageId, eventId);
            return View(model);
        }

        /// <summary>
        /// Handle POST events to update existing voyage events
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditVoyageEventViewModel model)
        {
            if (ModelState.IsValid)
            {
                _logger.LogDebug($"Editing voyage event: " + 
                    $"ID = {model.Id}, " +
                    $"Voyage ID = {model.VoyageId}, " + 
                    $"Date = {model.Date}, " + 
                    $"Port = {model.Port}, " + 
                    $"EventType = {model.EventType}");

                Port port = await _portClient.GetAsync(model.Port);
                _ = await _voyageEventClient.UpdateAsync(model.Id, model.VoyageId, model.EventType, port.Id, model.Date);
                return RedirectToAction("Index", "VoyageBuilder", new { id = model.VoyageId });
            }
            else
            {
                LogModelState();
            }

            return View(model);
        }

        /// <summary>
        /// Handle POST events to delete an existing voyage event
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(long id)
        {
            // Retrieve the item
            _logger.LogDebug($"Retrieving voyage event: ID = {id}");
            var evt = await _voyageEventClient.GetAsync(id);

            // Delete the item
            _logger.LogDebug($"Deleting voyage event: ID = {id}");
            await _voyageEventClient.DeleteAsync(id);

            // Redirect to the voyage builder for the specified voyage
            return RedirectToAction("Index", "VoyageBuilder", new { id = evt.VoyageId });
        }

        /// <summary>
        /// Build a voyage/event model
        /// </summary>
        /// <param name="voyageId"></param>
        /// <param name="eventId"></param>
        /// <returns></returns>
        private async Task<T> BuildModel<T>(long voyageId, long eventId) where T : VoyageEventViewModel, new()
        {
            // Load the voyage
            var voyage = await _voyageClient.GetAsync(voyageId);
            _logger.LogDebug($"Retrieved voyage : {voyage}");

            // Create the model
            var model = new T()
            {
                VoyageId = voyageId,
                VoyageNumber = voyage.Number
            };

            // Set the event properties on the model from the specified event
            var evt = eventId > 0 ? voyage.Events.FirstOrDefault(x => x.Id == eventId) : null;
            if (evt != null)
            {
                _logger.LogDebug($"Setting properties for event: {evt}");
                model.Id = evt.Id;
                model.Date = evt.Date;
                model.Port = evt.Port.Code;
                model.EventType = evt.EventType;
            }

            return model;
        }
    }
}