using System.Collections.Generic;
using System.Linq;
using ShippingRecorder.DataExchange.Entities;
using ShippingRecorder.Entities.Db;

namespace ShippingRecorder.DataExchange.Extensions
{
    public static class VesselExtensions
    {
        /// <summary>
        /// Return an exportable vessel from a vessel
        /// </summary>
        /// <param name="vessel"></param>
        /// <returns></returns>
        public static ExportableVessel ToExportable(this Vessel vessel)
            => new()
            {
                IMO = vessel.IMO,
                Built = vessel.Built,
                Draught = vessel.Draught,
                Length = vessel.Length,
                Beam = vessel.Beam,
                Tonnage = vessel.ActiveRegistrationHistory?.Tonnage,
                Passengers = vessel.ActiveRegistrationHistory?.Passengers,
                Crew = vessel.ActiveRegistrationHistory?.Crew,
                Decks = vessel.ActiveRegistrationHistory?.Decks,
                Cabins = vessel.ActiveRegistrationHistory?.Cabins,
                Name = vessel.ActiveRegistrationHistory?.Name,
                Callsign = vessel.ActiveRegistrationHistory?.Callsign,
                MMSI = vessel.ActiveRegistrationHistory?.MMSI,
                VesselType = vessel.ActiveRegistrationHistory?.VesselType?.Name,
                Flag = vessel.ActiveRegistrationHistory?.Flag?.Code,
                Operator = vessel.ActiveRegistrationHistory?.Operator?.Name
            };

        /// <summary>
        /// Return a collection of exportable vessels from a collection of vessels
        /// </summary>
        /// <param name="vessels"></param>
        /// <returns></returns>
        public static IEnumerable<ExportableVessel> ToExportable(this IEnumerable<Vessel> vessels)
        {
            var exportable = new List<ExportableVessel>();

            foreach (var vessel in vessels)
            {
                exportable.Add(vessel.ToExportable());
            }

            return exportable;
        }
    }
}
