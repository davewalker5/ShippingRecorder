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
    public class SightingExporter : ISightingExporter
    {
        private readonly IShippingRecorderFactory _factory;

        public event EventHandler<ExportEventArgs<ExportableSighting>> RecordExport;

        public SightingExporter(IShippingRecorderFactory factory)
            => _factory = factory;

        /// <summary>
        /// Export the meals to a CSV file
        /// </summary>
        /// <param name="file"></param>
        public async Task ExportAsync(string file)
        {
            var sightings = await _factory.Sightings.ListAsync(x => true, 1, int.MaxValue).ToListAsync();
            await ExportAsync(sightings, file);
        }

        /// <summary>
        /// Export a collection of meals to a CSV file
        /// </summary>
        /// <param name="locations"></param>
        /// <param name="file"></param>
#pragma warning disable CS1998
        public async Task ExportAsync(IEnumerable<Sighting> sightings, string file)
        {
            // Convert the sightings to exportable (flattened hierarchy) sightings
            var exportable = sightings.ToExportable();

            // Configure an exporter to export them
            var exporter = new CsvExporter<ExportableSighting>(ExportableEntityBase.DateFormat);
            exporter.RecordExport += OnRecordExported;

            // Export the records
            exporter.Export(exportable, file, ',');
        }
#pragma warning restore CS1998

        /// <summary>
        /// Handler for meal export notifications
        /// </summary>
        /// <param name="_"></param>
        /// <param name="e"></param>
        private void OnRecordExported(object _, ExportEventArgs<ExportableSighting> e)
        {
            RecordExport?.Invoke(this, e);
        }
    }
}
