using ShippingRecorder.BusinessLogic.Factory;
using ShippingRecorder.Entities.Db;
using ShippingRecorder.Entities.Interfaces;
using ShippingRecorder.Entities.Logging;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ShippingRecorder.Api.Controllers
{
    [Authorize]
    [ApiController]
    [ApiConventionType(typeof(DefaultApiConventions))]
    [Route("[controller]")]
    public class VoyageEventsController : ShippingRecorderApiController
    {
        public VoyageEventsController(ShippingRecorderFactory factory, IShippingRecorderLogger logger) : base(factory, logger)
        {
        }

        [HttpGet]
        [Route("{voyageId}/{pageNumber}/{pageSize}")]
        public async Task<ActionResult<List<VoyageEvent>>> GetVoyageEventsAsync(int voyageId, int pageNumber, int pageSize)
        {
            LogMessage(Severity.Debug, $"Retrieving list of events (page {pageNumber}, page size {pageSize})");

            List<VoyageEvent> voyageEvents = await Factory.VoyageEvents.ListAsync(x => x.VoyageId == voyageId, pageNumber, pageSize).ToListAsync();

            LogMessage(Severity.Debug, $"Retrieved {voyageEvents.Count} event(s)");

            if (!voyageEvents.Any())
            {
                return NoContent();
            }

            return voyageEvents;
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<ActionResult<VoyageEvent>> GetVoyageEventAsync(int id)
        {
            LogMessage(Severity.Debug, $"Retrieving event with ID {id}");

            VoyageEvent voyageEvent = await Factory.VoyageEvents.GetAsync(m => m.Id == id);

            if (voyageEvent == null)
            {
                LogMessage(Severity.Debug, $"Event with ID {id} not found");
                return NotFound();
            }

            return voyageEvent;
        }

        [HttpPost]
        [Route("")]
        public async Task<ActionResult<VoyageEvent>> AddVoyageEventAsync([FromBody] VoyageEvent template)
        {
            LogMessage(Severity.Debug,
                $"Adding event: " +
                $"Voyage ID = {template.VoyageId}, " +
                $"Port ID = {template.PortId}, " +
                $"Date = {template.Date}");

            VoyageEvent voyageEvent = await Factory.VoyageEvents.AddAsync(
                template.VoyageId,
                template.PortId,
                template.EventType,
                template.Date);

            LogMessage(Severity.Debug, $"Event added: {voyageEvent}");
            return voyageEvent;
        }

        [HttpPut]
        [Route("")]
        public async Task<ActionResult<VoyageEvent>> UpdateVoyageEventAsync([FromBody] VoyageEvent template)
        {
            LogMessage(Severity.Debug,
                $"Updating event: " +
                $"ID = {template.Id}, " +
                $"Voyage ID = {template.VoyageId}, " +
                $"Port ID = {template.PortId}, " +
                $"Date = {template.Date}");

            VoyageEvent voyageEvent = await Factory.VoyageEvents.UpdateAsync(
                template.Id,
                template.VoyageId,
                template.PortId,
                template.EventType,
                template.Date);

            LogMessage(Severity.Debug, $"Event updated: {voyageEvent}");
            return voyageEvent;
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> DeleteVoyageEventAsync(int id)
        {
            LogMessage(Severity.Debug, $"Deleting event: ID = {id}");
            await Factory.VoyageEvents.DeleteAsync(id);
            return Ok();
        }
    }
}
