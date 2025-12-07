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
    public class LocationsController : ShippingRecorderApiController
    {
        public LocationsController(ShippingRecorderFactory factory, IShippingRecorderLogger logger) : base(factory, logger)
        {
        }

        [HttpGet]
        [Route("{pageNumber}/{pageSize}")]
        public async Task<ActionResult<List<Location>>> GetLocationsAsync(int pageNumber, int pageSize)
        {
            LogMessage(Severity.Debug, $"Retrieving list of locations (page {pageNumber}, page size {pageSize})");

            List<Location> locations = await Factory.Locations.ListAsync(x => true, pageNumber, pageSize).ToListAsync();

            LogMessage(Severity.Debug, $"Retrieved {locations.Count} location(s)");

            if (!locations.Any())
            {
                return NoContent();
            }

            return locations;
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<ActionResult<Location>> GetLocationAsync(int id)
        {
            LogMessage(Severity.Debug, $"Retrieving location with ID {id}");

            Location location = await Factory.Locations.GetAsync(m => m.Id == id);

            if (location == null)
            {
                LogMessage(Severity.Debug, $"Location with ID {id} not found");
                return NotFound();
            }

            return location;
        }

        [HttpPost]
        [Route("")]
        public async Task<ActionResult<Location>> AddLocationAsync([FromBody] Location template)
        {
            LogMessage(Severity.Debug, $"Adding location: Name = {template.Name}");
            Location location = await Factory.Locations.AddAsync(template.Name);
            LogMessage(Severity.Debug, $"Location added: {location}");
            return location;
        }

        [HttpPut]
        [Route("")]
        public async Task<ActionResult<Location>> UpdateLocationAsync([FromBody] Location template)
        {
            LogMessage(Severity.Debug, $"Updating location: ID = {template.Id}, Name = {template.Name}");
            Location location = await Factory.Locations.UpdateAsync(template.Id, template.Name);
            LogMessage(Severity.Debug, $"Location updated: {location}");
            return location;
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> DeleteLocationAsync(int id)
        {
            LogMessage(Severity.Debug, $"Deleting location: ID = {id}");
            await Factory.Locations.DeleteAsync(id);
            return Ok();
        }
    }
}
