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
    public class PortsController : ShippingRecorderApiController
    {
        public PortsController(ShippingRecorderFactory factory, IShippingRecorderLogger logger) : base(factory, logger)
        {
        }

        [HttpGet]
        [Route("{countryId}/{pageNumber}/{pageSize}")]
        public async Task<ActionResult<List<Port>>> GetPortsAsync(int countryId, int pageNumber, int pageSize)
        {
            LogMessage(Severity.Debug, $"Retrieving list of ports (page {pageNumber}, page size {pageSize})");

            // If the country's 0 or less, just return all ports. Otherwise, return only ports for the specified
            // country ID
            Expression<Func<Port, bool>> predicate = x => countryId > 0 ? x.CountryId == countryId : true;
            List<Port> ports = await Factory.Ports.ListAsync(predicate, pageNumber, pageSize).ToListAsync();

            LogMessage(Severity.Debug, $"Retrieved {ports.Count} port(s)");

            if (!ports.Any())
            {
                return NoContent();
            }

            return ports;
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<ActionResult<Port>> GetPortAsync(int id)
        {
            LogMessage(Severity.Debug, $"Retrieving port with ID {id}");

            Port port = await Factory.Ports.GetAsync(m => m.Id == id);

            if (port == null)
            {
                LogMessage(Severity.Debug, $"Port with ID {id} not found");
                return NotFound();
            }

            return port;
        }

        [HttpGet]
        [Route("unlocode/{code}")]
        public async Task<ActionResult<Port>> GetPortAsync(string code)
        {
            LogMessage(Severity.Debug, $"Retrieving port with UN/LOCODE {code}");

            Port port = await Factory.Ports.GetAsync(x => x.Code == code);

            if (port == null)
            {
                LogMessage(Severity.Debug, $"Port with UN/LOCODE {code} not found");
                return NotFound();
            }

            return port;
        }

        [HttpPost]
        [Route("")]
        public async Task<ActionResult<Port>> AddPortAsync([FromBody] Port template)
        {
            LogMessage(Severity.Debug, $"Adding port: Country ID = {template.CountryId}, Code = {template.Code}, Name = {template.Name}");
            Port port = await Factory.Ports.AddAsync(template.CountryId, template.Code, template.Name);
            LogMessage(Severity.Debug, $"Port added: {port}");
            return port;
        }

        [HttpPut]
        [Route("")]
        public async Task<ActionResult<Port>> UpdatePortAsync([FromBody] Port template)
        {
            LogMessage(Severity.Debug, $"Updating port: ID = {template.Id}, Country ID = {template.CountryId}, Code = {template.Code}, Name = {template.Name}");
            Port port = await Factory.Ports.UpdateAsync(template.Id, template.CountryId, template.Code, template.Name);
            LogMessage(Severity.Debug, $"Port updated: {port}");
            return port;
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> DeletePortAsync(int id)
        {
            LogMessage(Severity.Debug, $"Deleting port: ID = {id}");
            await Factory.Ports.DeleteAsync(id);
            return Ok();
        }
    }
}
