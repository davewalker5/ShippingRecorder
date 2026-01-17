using System.Collections.Generic;
using ShippingRecorder.DataExchange.Entities;
using ShippingRecorder.Entities.Db;

namespace ShippingRecorder.DataExchange.Extensions
{
    public static class SightingExtensions
    {
        /// <summary>
        /// Return an exportable sighting from a sighting
        /// </summary>
        /// <param name="sighting"></param>
        /// <returns></returns>
        public static ExportableSighting ToExportable(this Sighting sighting)
            =>  new()
                {
                    Date = sighting.Date,
                    Location = sighting.Location.Name,
                    IMO = sighting.Vessel.IMO,
                    VoyageNumber = sighting.Voyage?.Number,
                    IsMyVoyage = sighting.IsMyVoyage
                };

        /// <summary>
        /// Return a collection of exportable sightings from a collection of sightings
        /// </summary>
        /// <param name="sightings"></param>
        /// <returns></returns>
        public static IEnumerable<ExportableSighting> ToExportable(this IEnumerable<Sighting> sightings)
        {
            var exportable = new List<ExportableSighting>();

            foreach (var sighting in sightings)
            {
                exportable.Add(sighting.ToExportable());
            }

            return exportable;
        }
    }
}
