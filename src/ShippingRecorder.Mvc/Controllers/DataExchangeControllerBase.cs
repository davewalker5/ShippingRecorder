using ShippingRecorder.Client.Interfaces;
using ShippingRecorder.Mvc.Enumerations;
using ShippingRecorder.Mvc.Interfaces;

namespace ShippingRecorder.Mvc.Controllers
{
    public abstract class DataExchangeControllerBase : ShippingRecorderControllerBase
    {
        private readonly Dictionary<DataExchangeType, string> _controllerMap = new()
        {
            { DataExchangeType.Countries, "Country" },
            { DataExchangeType.Locations, "Location" },
            { DataExchangeType.Operators, "Operator" },
            { DataExchangeType.Ports, "Port" },
            { DataExchangeType.Sightings, "Sighting" },
            { DataExchangeType.Vessels, "Vessel" },
            { DataExchangeType.VesselTypes, "VesselType" },
            { DataExchangeType.Voyages, "Voyage" }
        };

        protected readonly Dictionary<DataExchangeType, IImporter> _importers = new();
        protected readonly Dictionary<DataExchangeType, IExporter> _exporters = new();

        public DataExchangeControllerBase(
            ICountryClient countryClient,
            ILocationClient locationClient,
            IOperatorClient operatorClient,
            IPortClient portClient,
            ISightingClient sightingClient,
            IVesselClient vesselClient,
            IVesselTypeClient vesselTypeClient,
            IVoyageClient voyageClient,
            IPartialViewToStringRenderer renderer,
            ILogger logger) : base(renderer, logger)
        {
            _importers.Add(DataExchangeType.Countries, countryClient);
            _importers.Add(DataExchangeType.Locations, locationClient);
            _importers.Add(DataExchangeType.Operators, operatorClient);
            _importers.Add(DataExchangeType.Ports, portClient);
            _importers.Add(DataExchangeType.Sightings, sightingClient);
            _importers.Add(DataExchangeType.Vessels, vesselClient);
            _importers.Add(DataExchangeType.VesselTypes, vesselTypeClient);
            _importers.Add(DataExchangeType.Voyages, voyageClient);

            _exporters.Add(DataExchangeType.Countries, countryClient);
            _exporters.Add(DataExchangeType.Locations, locationClient);
            _exporters.Add(DataExchangeType.Operators, operatorClient);
            _exporters.Add(DataExchangeType.Sightings, sightingClient);
            _exporters.Add(DataExchangeType.Vessels, vesselClient);
            _exporters.Add(DataExchangeType.VesselTypes, vesselTypeClient);
            _exporters.Add(DataExchangeType.Voyages, voyageClient);
        }

        /// <summary>
        /// Return the name of the controller associated a given data exchange type
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        protected string ControllerName(DataExchangeType type)
            => _controllerMap[type];

        /// <summary>
        /// Return the data import API client associated with a given data exchange type
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        protected IImporter ImportClient(DataExchangeType type)
            => _importers[type];

        /// <summary>
        /// Return the data export API client associated with a given data exchange type
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        protected IExporter ExportClient(DataExchangeType type)
            => _exporters[type];
    }
}