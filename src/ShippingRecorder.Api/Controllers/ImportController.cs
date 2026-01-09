using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShippingRecorder.Api.Entities;
using ShippingRecorder.Api.Interfaces;

namespace ShippingRecorder.Api.Controllers
{
    [Authorize]
    [ApiController]
    [ApiConventionType(typeof(DefaultApiConventions))]
    [Route("[controller]")]
    public class ImportController : Controller
    {
        private readonly IBackgroundQueue<CountryImportWorkItem> _countryQueue;
        private readonly IBackgroundQueue<LocationImportWorkItem> _locationQueue;

        public ImportController(
            IBackgroundQueue<CountryImportWorkItem> countryQueue,
            IBackgroundQueue<LocationImportWorkItem> locationQueue)
        {
            _countryQueue = countryQueue;
            _locationQueue = locationQueue;
        }

        [HttpPost]
        [Route("countries")]
        public IActionResult ImportCountries([FromBody] CountryImportWorkItem item)
        {
            item.JobName = "Country Import";
            _countryQueue.Enqueue(item);
            return Accepted();
        }

        [HttpPost]
        [Route("locations")]
        public IActionResult ImportLocations([FromBody] LocationImportWorkItem item)
        {
            item.JobName = "Location Import";
            _locationQueue.Enqueue(item);
            return Accepted();
        }
    }
}
