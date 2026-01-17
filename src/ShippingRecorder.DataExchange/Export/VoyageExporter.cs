using ShippingRecorder.DataExchange.Entities;
using ShippingRecorder.DataExchange.Interfaces;
using ShippingRecorder.DataExchange.Extensions;
using ShippingRecorder.Entities.Interfaces;
using System;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;
using ShippingRecorder.Entities.Db;

namespace ShippingRecorder.DataExchange.Export
{
    public class VoyageExporter : IVoyageExporter
    {
        private readonly IShippingRecorderFactory _factory;

        public event EventHandler<ExportEventArgs<ExportableVoyage>> RecordExport;

        public VoyageExporter(IShippingRecorderFactory factory)
            => _factory = factory;

        /// <summary>
        /// Export the voyages to a CSV file
        /// </summary>
        /// <param name="file"></param>
        public async Task ExportAsync(string file)
        {
            var voyages = await _factory.Voyages.ListAsync(x => true, 1, int.MaxValue).ToListAsync();
            await ExportAsync(voyages, file);
        }

        /// <summary>
        /// Export a collection of voyages to a CSV file
        /// </summary>
        /// <param name="voyages"></param>
        /// <param name="file"></param>
#pragma warning disable CS1998
        public async Task ExportAsync(IEnumerable<Voyage> voyages, string file)
        {
            // Convert the voyages to exportable (flattened hierarchy) voyages
            var exportable = voyages.ToExportable();

            // Configure an exporter to export them
            var exporter = new CsvExporter<ExportableVoyage>(ExportableEntityBase.DateFormat);
            exporter.RecordExport += OnRecordExported;

            // Export the records
            exporter.Export(exportable, file, ',');
        }
#pragma warning restore CS1998

        /// <summary>
        /// Handler for voyage export notifications
        /// </summary>
        /// <param name="_"></param>
        /// <param name="e"></param>
        private void OnRecordExported(object _, ExportEventArgs<ExportableVoyage> e)
        {
            RecordExport?.Invoke(this, e);
        }
    }
}
