using System.Collections.Generic;
using System.Linq;
using ShippingRecorder.DataExchange.Entities;
using ShippingRecorder.Entities.Db;

namespace ShippingRecorder.DataExchange.Extensions
{
    public static class VoyageExtensions
    {
        /// <summary>
        /// Return an exportable voyage from a voyage. The result is a list of records, one per voyage event
        /// </summary>
        /// <param name="voyage"></param>
        /// <returns></returns>
        public static List<ExportableVoyage> ToExportable(this Voyage voyage)
            => voyage.Events
            .Select(e => new ExportableVoyage
            {
                Operator = voyage.Operator.Name,
                IMO = voyage.Vessel.IMO,
                Number = voyage.Number,
                EventType = e.EventType.ToString(),
                Port = e.Port.Code,
                Date = e.Date.ToString(ExportableEntityBase.DateFormat)
            })
            .ToList();

        /// <summary>
        /// Return a collection of exportable voyages from a collection of voyages
        /// </summary>
        /// <param name="voyages"></param>
        /// <returns></returns>
        public static IEnumerable<ExportableVoyage> ToExportable(this IEnumerable<Voyage> voyages)
        {
            var exportable = new List<ExportableVoyage>();

            foreach (var voyage in voyages)
            {
                exportable.AddRange(voyage.ToExportable());
            }

            return exportable;
        }
    }
}
