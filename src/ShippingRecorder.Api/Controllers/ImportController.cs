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
        private readonly IBackgroundQueue<OperatorImportWorkItem> _operatorQueue;
        private readonly IBackgroundQueue<PortImportWorkItem> _portQueue;
        private readonly IBackgroundQueue<VesselImportWorkItem> _vesselQueue;
        private readonly IBackgroundQueue<VesselTypeImportWorkItem> _vesselTypeQueue;
        private readonly IBackgroundQueue<SightingImportWorkItem> _sightingQueue;

        public ImportController(
            IBackgroundQueue<CountryImportWorkItem> countryQueue,
            IBackgroundQueue<LocationImportWorkItem> locationQueue,
            IBackgroundQueue<OperatorImportWorkItem> operatorQueue,
            IBackgroundQueue<PortImportWorkItem> portQueue,
            IBackgroundQueue<VesselImportWorkItem> vesselQueue,
            IBackgroundQueue<VesselTypeImportWorkItem> vesselTypeQueue,
            IBackgroundQueue<SightingImportWorkItem> sightingQueue)
        {
            _countryQueue = countryQueue;
            _locationQueue = locationQueue;
            _operatorQueue = operatorQueue;
            _portQueue = portQueue;
            _vesselQueue = vesselQueue;
            _vesselTypeQueue = vesselTypeQueue;
            _sightingQueue = sightingQueue;
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

        [HttpPost]
        [Route("operators")]
        public IActionResult ImportOperators([FromBody] OperatorImportWorkItem item)
        {
            item.JobName = "Operator Import";
            _operatorQueue.Enqueue(item);
            return Accepted();
        }

        [HttpPost]
        [Route("ports")]
        public IActionResult ImportPorts([FromBody] PortImportWorkItem item)
        {
            item.JobName = "Port Import";
            _portQueue.Enqueue(item);
            return Accepted();
        }

        [HttpPost]
        [Route("vessels")]
        public IActionResult ImportVessels([FromBody] VesselImportWorkItem item)
        {
            item.JobName = "Vessel Import";
            _vesselQueue.Enqueue(item);
            return Accepted();
        }

        [HttpPost]
        [Route("vesseltypes")]
        public IActionResult ImportVesselTypes([FromBody] VesselTypeImportWorkItem item)
        {
            item.JobName = "Vessel Type Import";
            _vesselTypeQueue.Enqueue(item);
            return Accepted();
        }

        [HttpPost]
        [Route("sightings")]
        public IActionResult ImportSightings([FromBody] SightingImportWorkItem item)
        {
            item.JobName = "Sighting Import";
            _sightingQueue.Enqueue(item);
            return Accepted();
        }
    }
}
