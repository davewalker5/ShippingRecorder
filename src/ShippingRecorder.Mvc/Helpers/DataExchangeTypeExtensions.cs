using ShippingRecorder.Mvc.Enumerations;

namespace ShippingRecorder.Mvc.Helpers
{
    public static class DataExchangeTypeExtensions
    {
        /// <summary>
        /// Convert a data exchange type to a descriptive string
        /// </summary>
        /// <param name="units"></param>
        /// <returns></returns>
        public static string ToName(this DataExchangeType type)
        {
            return type switch
            {
                DataExchangeType.None => "",
                DataExchangeType.Countries => "Countries",
                DataExchangeType.Locations => "Locations",
                DataExchangeType.Operators => "Operators",
                DataExchangeType.Ports => "Ports",
                DataExchangeType.Sightings => "Sightings",
                DataExchangeType.Vessels => "Vessels",
                DataExchangeType.VesselTypes => "Vessel Types",
                DataExchangeType.Voyages => "Voyages",
                _ => "",
            };
        }
    }
}