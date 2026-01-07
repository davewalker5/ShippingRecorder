using System.Collections.Generic;
using System.Linq;
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
                    IsMyVoyage = sighting.IsMyVoyage
                };

        /// <summary>
        /// Return a collection of exportable sightings from a collection of sightings
        /// </summary>
        /// <param name="people"></param>
        /// <returns></returns>
        public static IEnumerable<ExportableSighting> ToExportable(this IEnumerable<Sighting> people)
        {
            var exportable = new List<ExportableSighting>();

            foreach (var sighting in people)
            {
                exportable.Add(sighting.ToExportable());
            }

            return exportable;
        }

        /// <summary>
        /// Return a sighting from an exportable sighting
        /// </summary>
        /// <param name="sighting"></param>
        /// <param name="locations"></param>
        /// <param name="vessels"></param>
        /// <returns></returns>
        public static Sighting FromExportable(
            this ExportableSighting sighting,
            IEnumerable<Location> locations,
            IEnumerable<Vessel> vessels)
        {
            var location = locations.First(x => x.Name == sighting.Location);
            var vessel = vessels.First(x => x.IMO == sighting.IMO);
            return new()
            {
                Date = sighting.Date,
                LocationId = location.Id,
                Location = location,
                VesselId = vessel.Id,
                Vessel = vessel,
                IsMyVoyage = sighting.IsMyVoyage
            };
        }

        /// <summary>
        /// Return a collection of sightings from a collection of exportable sightings
        /// </summary>
        /// <param name="exportable"></param>
        /// <param name="locations"></param>
        /// <param name="vessels"></param>
        /// <returns></returns>
        public static IEnumerable<Sighting> FromExportable(
            this IEnumerable<ExportableSighting> exportable,
            IEnumerable<Location> locations,
            IEnumerable<Vessel> vessels)
        {
            var sightings = new List<Sighting>();

            foreach (var sighting in exportable)
            {
                sightings.Add(sighting.FromExportable(locations, vessels));
            }

            return sightings;
        }
    }
}
