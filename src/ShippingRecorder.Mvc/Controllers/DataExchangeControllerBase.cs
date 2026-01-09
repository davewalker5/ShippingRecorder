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
            { DataExchangeType.VesselTypes, "VesselType" }
        };

        protected readonly Dictionary<DataExchangeType, IImporterExporter> _clients = new();

        public DataExchangeControllerBase(
            ICountryClient countryClient,
            ILocationClient locationClient,
            IOperatorClient operatorClient,
            IPortClient portClient,
            ISightingClient sightingClient,
            IVesselClient vesselClient,
            IVesselTypeClient vesselTypeClient,
            IPartialViewToStringRenderer renderer,
            ILogger logger) : base(renderer, logger)
        {
            _clients.Add(DataExchangeType.Countries, countryClient);
            _clients.Add(DataExchangeType.Locations, locationClient);
            _clients.Add(DataExchangeType.Operators, operatorClient);
            _clients.Add(DataExchangeType.Ports, portClient);
            _clients.Add(DataExchangeType.Sightings, sightingClient);
            _clients.Add(DataExchangeType.Vessels, vesselClient);
            _clients.Add(DataExchangeType.VesselTypes, vesselTypeClient);
        }

        /// <summary>
        /// Return the name of the controller associated a given data exchange type
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        protected string ControllerName(DataExchangeType type)
            => _controllerMap[type];

        /// <summary>
        /// Return the API client associated with a given data exchange type
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        protected IImporterExporter Client(DataExchangeType type)
            => _clients[type];
    }
}