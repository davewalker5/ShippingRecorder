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
    public class CountryExporter : ICountryExporter
    {
        private readonly IShippingRecorderFactory _factory;

        public event EventHandler<ExportEventArgs<ExportableCountry>> RecordExport;

        public CountryExporter(IShippingRecorderFactory factory)
            => _factory = factory;

        /// <summary>
        /// Export the countrys to a CSV file
        /// </summary>
        /// <param name="file"></param>
        public async Task ExportAsync(string file)
        {
            var countrys = await _factory.Countries.ListAsync(x => true, 1, int.MaxValue).ToListAsync();
            await ExportAsync(countrys, file);
        }

        /// <summary>
        /// Export a collection of countrys to a CSV file
        /// </summary>
        /// <param name="countries"></param>
        /// <param name="file"></param>
#pragma warning disable CS1998
        public async Task ExportAsync(IEnumerable<Country> countries, string file)
        {
            // Convert the countrys to exportable (flattened hierarchy) countrys
            var exportable = countries.ToExportable();

            // Configure an exporter to export them
            var exporter = new CsvExporter<ExportableCountry>(ExportableEntityBase.DateFormat);
            exporter.RecordExport += OnRecordExported;

            // Export the records
            exporter.Export(exportable, file, ',');
        }
#pragma warning restore CS1998

        /// <summary>
        /// Handler for country export notifications
        /// </summary>
        /// <param name="_"></param>
        /// <param name="e"></param>
        private void OnRecordExported(object _, ExportEventArgs<ExportableCountry> e)
        {
            RecordExport?.Invoke(this, e);
        }
    }
}
