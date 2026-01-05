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
    public class RegistrationHistoryController : ShippingRecorderApiController
    {
        public RegistrationHistoryController(ShippingRecorderFactory factory, IShippingRecorderLogger logger) : base(factory, logger)
        {
        }

        [HttpGet]
        [Route("{vesselId}/{pageNumber}/{pageSize}")]
        public async Task<ActionResult<List<RegistrationHistory>>> GetRegistrationHistoryAsync(int vesselId, int pageNumber, int pageSize)
        {
            LogMessage(Severity.Debug, $"Retrieving list of registration histories (page {pageNumber}, page size {pageSize})");

            List<RegistrationHistory> details = await Factory.RegistrationHistory
                                                     .ListAsync(x => x.VesselId == vesselId, pageNumber, pageSize)
                                                     .ToListAsync();

            LogMessage(Severity.Debug, $"Retrieved {details.Count} registration detail(s)");

            if (!details.Any())
            {
                return NoContent();
            }

            return details;
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<ActionResult<RegistrationHistory>> GetRegistrationHistoryAsync(int id)
        {
            LogMessage(Severity.Debug, $"Retrieving registration history with ID {id}");

            RegistrationHistory details = await Factory.RegistrationHistory.GetAsync(m => m.Id == id);

            if (details == null)
            {
                LogMessage(Severity.Debug, $"Registration history with ID {id} not found");
                return NotFound();
            }

            return details;
        }

        [HttpPost]
        [Route("")]
        public async Task<ActionResult<RegistrationHistory>> AddRegistrationHistoryAsync([FromBody] RegistrationHistory template)
        {
            LogMessage(Severity.Debug,
                $"Adding registration history: " +
                $"Vessel ID = {template.VesselId}, " +
                $"Vessel Type ID = {template.VesselTypeId}, " +
                $"Flag ID = {template.FlagId}, " +
                $"Operator ID = {template.OperatorId}, " +
                $"Name = {template.Name}, " +
                $"Callsign = {template.Callsign}, " +
                $"MMSI = {template.MMSI}, " +
                $"Tonnage = {template.Tonnage}, " +
                $"Passengers = {template.Passengers}, " +
                $"Crew = {template.Crew}, " +
                $"Decks = {template.Decks}, " +
                $"Cabins = {template.Cabins}");

            RegistrationHistory history = await Factory.RegistrationHistory.AddAsync(
                template.VesselId,
                template.VesselTypeId,
                template.FlagId,
                template.OperatorId,
                template.Name,
                template.Callsign,
                template.MMSI,
                template.Tonnage,
                template.Passengers,
                template.Crew,
                template.Decks,
                template.Cabins);

            LogMessage(Severity.Debug, $"Registration history added: {history}");
            return history;
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> DeactivateRegistrationHistoryAsync(int id)
        {
            LogMessage(Severity.Debug, $"Deleting vessel: ID = {id}");
            await Factory.RegistrationHistory.Deactivate(id);
            return Ok();
        }

        [HttpGet]
        [Route("vessel/{vesselId}")]
        public async Task<ActionResult<RegistrationHistory>> GetActiveRegistrationHistoryForVesselAsync(int vesselId)
        {
            LogMessage(Severity.Debug, $"Retrieving current registration history for vessel with ID {vesselId}");

            var details = await Factory.RegistrationHistory.GetAsync(x => x.VesselId == vesselId && x.IsActive);

            if (details == null)
            {
                return NoContent();
            }

            return details;
        }
    }
}
