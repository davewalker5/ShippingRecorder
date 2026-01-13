using ShippingRecorder.Entities.Db;

namespace ShippingRecorder.Enumerations.Extensions
{
    public static class VoyageEventTypeExtensions
    {
        /// <summary>
        /// Convert a data exchange type to a descriptive string
        /// </summary>
        /// <param name="units"></param>
        /// <returns></returns>
        public static string ToName(this VoyageEventType type)
        {
            return type switch
            {
                VoyageEventType.None => "",
                VoyageEventType.Depart => "Depart",
                VoyageEventType.Arrive => "Arrive",
                _ => "",
            };
        }
    }
}