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
        private readonly IBackgroundQueue<LocationImportWorkItem> _locationQueue;

        public ImportController(IBackgroundQueue<LocationImportWorkItem> locationQueue)
        {
            _locationQueue = locationQueue;
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
