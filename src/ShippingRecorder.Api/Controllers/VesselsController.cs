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
    public class VesselsController : ShippingRecorderApiController
    {
        public VesselsController(ShippingRecorderFactory factory, IShippingRecorderLogger logger) : base(factory, logger)
        {
        }

        [HttpGet]
        [Route("{pageNumber}/{pageSize}")]
        public async Task<ActionResult<List<Vessel>>> GetVesselsAsync(int pageNumber, int pageSize)
        {
            LogMessage(Severity.Debug, $"Retrieving list of vessels (page {pageNumber}, page size {pageSize})");

            List<Vessel> vessels = await Factory.Vessels.ListAsync(x => true, pageNumber, pageSize).ToListAsync();

            LogMessage(Severity.Debug, $"Retrieved {vessels.Count} vessel(s)");

            if (!vessels.Any())
            {
                return NoContent();
            }

            return vessels;
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<ActionResult<Vessel>> GetVesselByIdAsync(int id)
        {
            LogMessage(Severity.Debug, $"Retrieving vessel with ID {id}");

            Vessel vessel = await Factory.Vessels.GetAsync(m => m.Id == id);

            if (vessel == null)
            {
                LogMessage(Severity.Debug, $"Vessel with ID {id} not found");
                return NotFound();
            }

            return vessel;
        }

        [HttpGet]
        [Route("identifier/{identifier}")]
        public async Task<ActionResult<Vessel>> GetVesselByIdentifierAsync(string identifier)
        {
            LogMessage(Severity.Debug, $"Retrieving vessel with identifier {identifier}");

            Vessel vessel = await Factory.Vessels.GetAsync(m => m.Identifier == identifier);

            if (vessel == null)
            {
                LogMessage(Severity.Debug, $"Vessel with identifier {identifier} not found");
                return NotFound();
            }

            return vessel;
        }

        [HttpPost]
        [Route("")]
        public async Task<ActionResult<Vessel>> AddVesselAsync([FromBody] Vessel template)
        {
            LogMessage(Severity.Debug,
                $"Adding vessel: " +
                $"Identifier = {template.Identifier}, " +
                $"Is IMO = {template.IsIMO}, " +
                $"Built = {template.Built}, " +
                $"Draught = {template.Draught}, " +
                $"Length = {template.Length}, " +
                $"Beam = {template.Beam}");

            Vessel vessel = await Factory.Vessels.AddAsync(template.Identifier, template.IsIMO, template.Built, template.Draught, template.Length, template.Beam);
            LogMessage(Severity.Debug, $"Vessel added: {vessel}");
            return vessel;
        }

        [HttpPut]
        [Route("")]
        public async Task<ActionResult<Vessel>> UpdateVesselAsync([FromBody] Vessel template)
        {
            LogMessage(Severity.Debug,
                $"Adding vessel: " +
                $"ID = {template.Id}, " +
                $"Identifier = {template.Identifier}, " +
                $"Is IMO = {template.IsIMO}, " +
                $"Built = {template.Built}, " +
                $"Draught = {template.Draught}, " +
                $"Length = {template.Length}, " +
                $"Beam = {template.Beam}");

            Vessel vessel = await Factory.Vessels.UpdateAsync(template.Id, template.Identifier, template.IsIMO, template.Built, template.Draught, template.Length, template.Beam);
            LogMessage(Severity.Debug, $"Vessel updated: {vessel}");
            return vessel;
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> DeleteVesselAsync(int id)
        {
            LogMessage(Severity.Debug, $"Deleting vessel: ID = {id}");
            await Factory.Vessels.DeleteAsync(id);
            return Ok();
        }
    }
}
