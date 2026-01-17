using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShippingRecorder.Api.Interfaces;
using ShippingRecorder.Api.Entities;
using ShippingRecorder.Entities.Jobs;

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
        private readonly IBackgroundQueue<PortExportWorkItem> _portQueue;
        private readonly IBackgroundQueue<SightingExportWorkItem> _sightingQueue;
        private readonly IBackgroundQueue<VesselExportWorkItem> _vesselQueue;
        private readonly IBackgroundQueue<VesselTypeExportWorkItem> _vesselTypeQueue;
        private readonly IBackgroundQueue<VoyageExportWorkItem> _voyageQueue;

        public ExportController(
            IBackgroundQueue<CountryExportWorkItem> countryQueue,
            IBackgroundQueue<LocationExportWorkItem> locationQueue,
            IBackgroundQueue<OperatorExportWorkItem> operatorQueue,
            IBackgroundQueue<PortExportWorkItem> portQueue,
            IBackgroundQueue<SightingExportWorkItem> sightingQueue,
            IBackgroundQueue<VesselExportWorkItem> vesselQueue,
            IBackgroundQueue<VesselTypeExportWorkItem> vesselTypeQueue,
            IBackgroundQueue<VoyageExportWorkItem> voyageQueue)
        {
            _countryQueue = countryQueue;
            _locationQueue = locationQueue;
            _operatorQueue = operatorQueue;
            _portQueue = portQueue;
            _sightingQueue = sightingQueue;
            _vesselQueue = vesselQueue;
            _vesselTypeQueue = vesselTypeQueue;
            _voyageQueue = voyageQueue;
        }

        [HttpPost]
        [Route("all")]
        public IActionResult ExportAll([FromBody] AllExportWorkItem item)
        {
            // Get the file name without the extension or path
            var fileName = Path.GetFileNameWithoutExtension(item.FileName);

            // Queue exports for each data type
            _countryQueue.Enqueue(BuildWorkItem<CountryExportWorkItem>("Countries", fileName));
            _locationQueue.Enqueue(BuildWorkItem<LocationExportWorkItem>("Locations", fileName));
            _operatorQueue.Enqueue(BuildWorkItem<OperatorExportWorkItem>("Operators", fileName));
            _portQueue.Enqueue(BuildWorkItem<PortExportWorkItem>("Ports", fileName));
            _sightingQueue.Enqueue(BuildWorkItem<SightingExportWorkItem>("Sightings", fileName));
            _vesselQueue.Enqueue(BuildWorkItem<VesselExportWorkItem>("Vessels", fileName));
            _vesselTypeQueue.Enqueue(BuildWorkItem<VesselTypeExportWorkItem>("Vessel-Types", fileName));
            _voyageQueue.Enqueue(BuildWorkItem<VoyageExportWorkItem>("Voyages", fileName));

            return Accepted();
        }

        [HttpPost]
        [Route("countries")]
        public IActionResult ExportCountries([FromBody] CountryExportWorkItem item)
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
        [Route("ports")]
        public IActionResult ExportPorts([FromBody] PortExportWorkItem item)
        {
            item.JobName = "Port Export";
            _portQueue.Enqueue(item);
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
        [Route("voyages")]
        public IActionResult ExportVoyages([FromBody] VoyageExportWorkItem item)
        {
            item.JobName = "Voyage Export";
            _voyageQueue.Enqueue(item);
            return Accepted();
        }

        /// <summary>
        /// Helper method to build a work item for the specified entity type
        /// </summary>
        /// <param name="entityName"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        private T BuildWorkItem<T>(string entityName, string fileName) where T : ExportWorkItem, new()
            => new()
            {
                JobName = $"{entityName} Export",
                FileName = $"{fileName}-{entityName}.csv"
            };
    }
}
