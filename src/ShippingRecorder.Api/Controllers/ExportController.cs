using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShippingRecorder.Api.Interfaces;
using ShippingRecorder.Api.Entities;

namespace HealthTracker.Api.Controllers
{
    [Authorize]
    [ApiController]
    [ApiConventionType(typeof(DefaultApiConventions))]
    [Route("[controller]")]
    public class ExportController : Controller
    {
        private readonly IBackgroundQueue<CountryExportWorkItem> _countryQueue;
        private readonly IBackgroundQueue<LocationExportWorkItem> _locationQueue;
        private readonly IBackgroundQueue<OperatorExportWorkItem> _operatorQueue;
        private readonly IBackgroundQueue<VesselExportWorkItem> _vesselQueue;
        private readonly IBackgroundQueue<VesselTypeExportWorkItem> _vesselTypeQueue;
        private readonly IBackgroundQueue<SightingExportWorkItem> _sightingQueue;

        public ExportController(
            IBackgroundQueue<CountryExportWorkItem> countryQueue,
            IBackgroundQueue<LocationExportWorkItem> locationQueue,
            IBackgroundQueue<OperatorExportWorkItem> operatorQueue,
            IBackgroundQueue<VesselExportWorkItem> vesselQueue,
            IBackgroundQueue<VesselTypeExportWorkItem> vesselTypeQueue,
            IBackgroundQueue<SightingExportWorkItem> sightingQueue)
        {
            _countryQueue = countryQueue;
            _locationQueue = locationQueue;
            _operatorQueue = operatorQueue;
            _vesselQueue = vesselQueue;
            _vesselTypeQueue = vesselTypeQueue;
            _sightingQueue = sightingQueue;
        }

        [HttpPost]
        [Route("countries")]
        public IActionResult ExportCountris([FromBody] CountryExportWorkItem item)
        {
            item.JobName = "Country Export";
            _countryQueue.Enqueue(item);
            return Accepted();
        }

        [HttpPost]
        [Route("locations")]
        public IActionResult ExportLocations([FromBody] LocationExportWorkItem item)
        {
            item.JobName = "Location Export";
            _locationQueue.Enqueue(item);
            return Accepted();
        }

        [HttpPost]
        [Route("operators")]
        public IActionResult ExportOperators([FromBody] OperatorExportWorkItem item)
        {
            item.JobName = "Operator Export";
            _operatorQueue.Enqueue(item);
            return Accepted();
        }

        [HttpPost]
        [Route("vessels")]
        public IActionResult ExportVessels([FromBody] VesselExportWorkItem item)
        {
            item.JobName = "Vessel Export";
            _vesselQueue.Enqueue(item);
            return Accepted();
        }

        [HttpPost]
        [Route("vesseltypes")]
        public IActionResult ExportVesselTypes([FromBody] VesselTypeExportWorkItem item)
        {
            item.JobName = "Vessel Type Export";
            _vesselTypeQueue.Enqueue(item);
            return Accepted();
        }

        [HttpPost]
        [Route("sightings")]
        public IActionResult ExportSightings([FromBody] SightingExportWorkItem item)
        {
            item.JobName = "Sighting Export";
            _sightingQueue.Enqueue(item);
            return Accepted();
        }
    }
}
