using ShippingRecorder.BusinessLogic.Factory;
using ShippingRecorder.Entities.Db;
using ShippingRecorder.Entities.Interfaces;
using ShippingRecorder.Entities.Logging;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Web;

namespace ShippingRecorder.Api.Controllers
{
    [Authorize]
    [ApiController]
    [ApiConventionType(typeof(DefaultApiConventions))]
    [Route("[controller]")]
    public class SightingsController : ShippingRecorderApiController
    {
        private const string DateTimeFormat = "yyyy-MM-dd H:mm:ss";

        public SightingsController(ShippingRecorderFactory factory, IShippingRecorderLogger logger) : base(factory, logger)
        {
        }

        [HttpGet]
        [Route("{pageNumber}/{pageSize}")]
        public async Task<ActionResult<List<Sighting>>> GetSightingsAsync(int pageNumber, int pageSize)
        {
            LogMessage(Severity.Debug, $"Retrieving list of sightings (page {pageNumber}, page size {pageSize})");

            List<Sighting> sightings = await Factory.Sightings.ListAsync(x => true, pageNumber, pageSize).ToListAsync();

            LogMessage(Severity.Debug, $"Retrieved {sightings.Count} sighting(s)");

            if (!sightings.Any())
            {
                return NoContent();
            }

            return sightings;
        }
        
        [HttpGet]
        [Route("vessel/{vesselId}/{pageNumber}/{pageSize}")]
        public async Task<ActionResult<List<Sighting>>> GetSightingsByVesselAsync(int vesselId, int pageNumber, int pageSize)
        {
            List<Sighting> sightings = await Factory.Sightings
                                                     .ListAsync(s => s.VesselId == vesselId, pageNumber, pageSize)
                                                     .ToListAsync();

            if (!sightings.Any())
            {
                return NoContent();
            }

            return sightings;
        }

        [HttpGet]
        [Route("location/{locationId}/{start}/{end}/{pageNumber}/{pageSize}")]
        public async Task<ActionResult<List<Sighting>>> GetSightingsByLocationAsync(int locationId, string start, string end, int pageNumber, int pageSize)
        {
            DateTime startDate = DateTime.ParseExact(HttpUtility.UrlDecode(start), DateTimeFormat, null);
            DateTime endDate = DateTime.ParseExact(HttpUtility.UrlDecode(end), DateTimeFormat, null);

            List<Sighting> sightings = await Factory.Sightings
                                                     .ListAsync(s => (s.LocationId == locationId) &&
                                                                     (s.Date >= startDate) &&
                                                                     (s.Date <= endDate), pageNumber, pageSize)
                                                     .ToListAsync();

            if (!sightings.Any())
            {
                return NoContent();
            }

            return sightings;
        }

        [HttpGet]
        [Route("date/{start}/{end}/{pageNumber}/{pageSize}")]
        public async Task<ActionResult<List<Sighting>>> GetSightingsByDateAsync(string start, string end, int pageNumber, int pageSize)
        {
            DateTime startDate = DateTime.ParseExact(HttpUtility.UrlDecode(start), DateTimeFormat, null);
            DateTime endDate = DateTime.ParseExact(HttpUtility.UrlDecode(end), DateTimeFormat, null);

            List<Sighting> sightings = await Factory.Sightings
                                                     .ListAsync(s => (s.Date >= startDate) &&
                                                                     (s.Date <= endDate), pageNumber, pageSize)
                                                     .ToListAsync();

            if (!sightings.Any())
            {
                return NoContent();
            }

            return sightings;
        }

        [HttpGet]
        [Route("mine/{pageNumber}/{pageSize}")]
        public async Task<ActionResult<List<Sighting>>> GetMyVoyagesAsync(int pageNumber, int pageSize)
        {
            List<Sighting> sightings = await Factory.Sightings
                                                     .ListAsync(s => s.IsMyVoyage, pageNumber, pageSize)
                                                     .ToListAsync();

            if (!sightings.Any())
            {
                return NoContent();
            }

            return sightings;
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<ActionResult<Sighting>> GetSightingAsync(int id)
        {
            LogMessage(Severity.Debug, $"Retrieving sighting with ID {id}");

            Sighting sighting = await Factory.Sightings.GetAsync(m => m.Id == id);

            if (sighting == null)
            {
                LogMessage(Severity.Debug, $"Sighting with ID {id} not found");
                return NotFound();
            }

            return sighting;
        }

        [HttpPost]
        [Route("")]
        public async Task<ActionResult<Sighting>> AddSightingAsync([FromBody] Sighting template)
        {
            LogMessage(Severity.Debug,
                $"Adding sighting: " + 
                $"Location ID = {template.LocationId}, " +
                $"Voyage ID = {template.VoyageId}, " +
                $"Vessel ID = {template.VesselId}, " +
                $"Date = {template.Date}, " +
                $"Is My Voyage = {template.IsMyVoyage}");

            Sighting sighting = await Factory.Sightings.AddAsync(
                template.LocationId,
                template.VoyageId,
                template.VesselId,
                template.Date,
                template.IsMyVoyage);

            LogMessage(Severity.Debug, $"Sighting added: {sighting}");
            return sighting;
        }

        [HttpPut]
        [Route("")]
        public async Task<ActionResult<Sighting>> UpdateSightingAsync([FromBody] Sighting template)
        {
            LogMessage(Severity.Debug,
                $"Updating sighting: " +
                $"ID = {template.Id}, " +
                $"Location ID = {template.LocationId}, " +
                $"Voyage ID = {template.VoyageId}, " +
                $"Vessel ID = {template.VesselId}, " +
                $"Date = {template.Date}, " +
                $"Is My Voyage = {template.IsMyVoyage}");

            Sighting sighting = await Factory.Sightings.UpdateAsync(
                template.Id,
                template.LocationId,
                template.VoyageId,
                template.VesselId,
                template.Date,
                template.IsMyVoyage);

            LogMessage(Severity.Debug, $"Sighting updated: {sighting}");
            return sighting;
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> DeleteSightingAsync(int id)
        {
            LogMessage(Severity.Debug, $"Deleting sighting: ID = {id}");
            await Factory.Sightings.DeleteAsync(id);
            return Ok();
        }
    }
}
