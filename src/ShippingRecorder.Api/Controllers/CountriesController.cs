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
    public class CountriesController : ShippingRecorderApiController
    {
        public CountriesController(ShippingRecorderFactory factory, IShippingRecorderLogger logger) : base(factory, logger)
        {
        }

        [HttpGet]
        [Route("{pageNumber}/{pageSize}")]
        public async Task<ActionResult<List<Country>>> GetCountriesAsync(int pageNumber, int pageSize)
        {
            LogMessage(Severity.Debug, $"Retrieving list of countries (page {pageNumber}, page size {pageSize})");

            List<Country> countries = await Factory.Countries.ListAsync(x => true, pageNumber, pageSize).ToListAsync();

            LogMessage(Severity.Debug, $"Retrieved {countries.Count} countries");

            if (!countries.Any())
            {
                return NoContent();
            }

            return countries;
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<ActionResult<Country>> GetCountryAsync(int id)
        {
            LogMessage(Severity.Debug, $"Retrieving country with ID {id}");

            Country country = await Factory.Countries.GetAsync(m => m.Id == id);

            if (country == null)
            {
                LogMessage(Severity.Debug, $"Country with ID {id} not found");
                return NotFound();
            }

            return country;
        }

        [HttpPost]
        [Route("")]
        public async Task<ActionResult<Country>> AddCountryAsync([FromBody] Country template)
        {
            LogMessage(Severity.Debug, $"Adding country: Code = {template.Code}, Name = {template.Name}");
            Country country = await Factory.Countries.AddAsync(template.Code, template.Name);
            LogMessage(Severity.Debug, $"Country added: {country}");
            return country;
        }

        [HttpPut]
        [Route("")]
        public async Task<ActionResult<Country>> UpdateCountryAsync([FromBody] Country template)
        {
            LogMessage(Severity.Debug, $"Updating country: ID = {template.Id}, Code = {template.Code}, Name = {template.Name}");
            Country country = await Factory.Countries.UpdateAsync(template.Id, template.Code, template.Name);
            LogMessage(Severity.Debug, $"Country updated: {country}");
            return country;
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> DeleteCountryAsync(int id)
        {
            LogMessage(Severity.Debug, $"Deleting country: ID = {id}");
            await Factory.Countries.DeleteAsync(id);
            return Ok();
        }
    }
}
