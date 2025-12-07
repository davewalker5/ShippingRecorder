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
    public class VesselTypesController : ShippingRecorderApiController
    {
        public VesselTypesController(ShippingRecorderFactory factory, IShippingRecorderLogger logger) : base(factory, logger)
        {
        }

        [HttpGet]
        [Route("{pageNumber}/{pageSize}")]
        public async Task<ActionResult<List<VesselType>>> GetVesselTypesAsync(int pageNumber, int pageSize)
        {
            LogMessage(Severity.Debug, $"Retrieving list of vessel types (page {pageNumber}, page size {pageSize})");

            List<VesselType> vesselTypes = await Factory.VesselTypes.ListAsync(x => true, pageNumber, pageSize).ToListAsync();

            LogMessage(Severity.Debug, $"Retrieved {vesselTypes.Count} vessel type(s)");

            if (!vesselTypes.Any())
            {
                return NoContent();
            }

            return vesselTypes;
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<ActionResult<VesselType>> GetVesselTypeAsync(int id)
        {
            LogMessage(Severity.Debug, $"Retrieving vessel type with ID {id}");

            VesselType vesselType = await Factory.VesselTypes.GetAsync(m => m.Id == id);

            if (vesselType == null)
            {
                LogMessage(Severity.Debug, $"Vessel type with ID {id} not found");
                return NotFound();
            }

            return vesselType;
        }

        [HttpPost]
        [Route("")]
        public async Task<ActionResult<VesselType>> AddVesselTypeAsync([FromBody] VesselType template)
        {
            LogMessage(Severity.Debug, $"Adding vessel type: Name = {template.Name}");
            VesselType vesselType = await Factory.VesselTypes.AddAsync(template.Name);
            LogMessage(Severity.Debug, $"Vessel type added: {vesselType}");
            return vesselType;
        }

        [HttpPut]
        [Route("")]
        public async Task<ActionResult<VesselType>> UpdateVesselTypeAsync([FromBody] VesselType template)
        {
            LogMessage(Severity.Debug, $"Updating vessel type: ID = {template.Id}, Name = {template.Name}");
            VesselType vesselType = await Factory.VesselTypes.UpdateAsync(template.Id, template.Name);
            LogMessage(Severity.Debug, $"Vessel type updated: {vesselType}");
            return vesselType;
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> DeleteVesselTypeAsync(int id)
        {
            LogMessage(Severity.Debug, $"Deleting vessel type: ID = {id}");
            await Factory.VesselTypes.DeleteAsync(id);
            return Ok();
        }
    }
}
