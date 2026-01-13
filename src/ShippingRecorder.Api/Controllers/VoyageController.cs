using ShippingRecorder.BusinessLogic.Factory;
using ShippingRecorder.Entities.Db;
using ShippingRecorder.Entities.Interfaces;
using ShippingRecorder.Entities.Logging;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq.Expressions;

namespace ShippingRecorder.Api.Controllers
{
    [Authorize]
    [ApiController]
    [ApiConventionType(typeof(DefaultApiConventions))]
    [Route("[controller]")]
    public class VoyagesController : ShippingRecorderApiController
    {
        public VoyagesController(ShippingRecorderFactory factory, IShippingRecorderLogger logger) : base(factory, logger)
        {
        }

        [HttpGet]
        [Route("{operatorId}/{pageNumber}/{pageSize}")]
        public async Task<ActionResult<List<Voyage>>> GetVoyagesAsync(int operatorId, int pageNumber, int pageSize)
        {
            LogMessage(Severity.Debug, $"Retrieving list of voyages (page {pageNumber}, page size {pageSize})");

            // If the operator's 0 or less, just return all voyages. Otherwise, return only voyages for the specified
            // operator ID
            Expression<Func<Voyage, bool>> predicate = x => operatorId > 0 ? x.Id == operatorId : true;
            List<Voyage> voyages = await Factory.Voyages.ListAsync(predicate, pageNumber, pageSize).ToListAsync();

            LogMessage(Severity.Debug, $"Retrieved {voyages.Count} voyage(s)");

            if (!voyages.Any())
            {
                return NoContent();
            }

            return voyages;
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<ActionResult<Voyage>> GetVoyageAsync(int id)
        {
            LogMessage(Severity.Debug, $"Retrieving voyage with ID {id}");

            Voyage voyage = await Factory.Voyages.GetAsync(m => m.Id == id);

            if (voyage == null)
            {
                LogMessage(Severity.Debug, $"Voyage with ID {id} not found");
                return NotFound();
            }

            return voyage;
        }

        [HttpPost]
        [Route("")]
        public async Task<ActionResult<Voyage>> AddVoyageAsync([FromBody] Voyage template)
        {
            LogMessage(Severity.Debug, $"Adding voyage: Operator ID = {template.OperatorId}, Number = {template.Number}");
            Voyage voyage = await Factory.Voyages.AddAsync(template.OperatorId, template.VesselId, template.Number);
            LogMessage(Severity.Debug, $"Voyage added: {voyage}");
            return voyage;
        }

        [HttpPut]
        [Route("")]
        public async Task<ActionResult<Voyage>> UpdateVoyageAsync([FromBody] Voyage template)
        {
            LogMessage(Severity.Debug, $"Updating voyage: ID = {template.Id}, Number = {template.Number}");
            Voyage voyage = await Factory.Voyages.UpdateAsync(template.Id, template.OperatorId, template.VesselId, template.Number);
            LogMessage(Severity.Debug, $"Voyage updated: {voyage}");
            return voyage;
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> DeleteVoyageAsync(int id)
        {
            LogMessage(Severity.Debug, $"Deleting voyage: ID = {id}");
            await Factory.Voyages.DeleteAsync(id);
            return Ok();
        }
    }
}
