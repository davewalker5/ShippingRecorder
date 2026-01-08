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

        /// <summary>
        /// Return a vessel from an exportable vessel
        /// </summary>
        /// <param name="vessel"></param>
        /// <param name="vesselTypes"></param>
        /// <param name="countries"></param>
        /// <param name="operators"></param>
        /// <returns></returns>
        public static Vessel FromExportable(
            this ExportableVessel vessel,
            IEnumerable<VesselType> vesselTypes,
            IEnumerable<Country> countries,
            IEnumerable<Operator> operators)
        {
            var vesselType = vesselTypes.First(x => x.Name == vessel.VesselType);
            var flag = countries.First(x => x.Code == vessel.Flag);
            var op = operators.First(x => x.Name == vessel.Operator);
            return new()
            {
                IMO = vessel.IMO,
                Built = vessel.Built,
                Draught = vessel.Draught,
                Length = vessel.Length,
                Beam = vessel.Beam,
                RegistrationHistory = [
                    new()
                    {
                        Tonnage = vessel.Tonnage,
                        Passengers = vessel.Passengers,
                        Crew = vessel.Crew,
                        Decks = vessel.Decks,
                        Cabins = vessel.Cabins,
                        Name = vessel.Name,
                        Callsign = vessel.Callsign,
                        MMSI = vessel.MMSI,
                        VesselType = vesselType,
                        VesselTypeId = vesselType.Id,
                        Flag = flag,
                        FlagId = flag.Id,
                        Operator = op,
                        OperatorId = op.Id
                    }
                ]
            };
        }

        /// <summary>
        /// Return a collection of vessels from a collection of exportable vessels
        /// </summary>
        /// <param name="exportable"></param>
        /// <param name="vesselTypes"></param>
        /// <param name="countries"></param>
        /// <param name="operators"></param>
        /// <returns></returns>
        public static IEnumerable<Vessel> FromExportable(
            this IEnumerable<ExportableVessel> exportable,
            IEnumerable<VesselType> vesselTypes,
            IEnumerable<Country> countries,
            IEnumerable<Operator> operators)
        {
            var vessels = new List<Vessel>();

            foreach (var vessel in exportable)
            {
                vessels.Add(vessel.FromExportable(vesselTypes, countries, operators));
            }

            return vessels;
        }
    }
}
