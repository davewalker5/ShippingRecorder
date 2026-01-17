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
    public class PortExporter : IPortExporter
    {
        private readonly IShippingRecorderFactory _factory;

        public event EventHandler<ExportEventArgs<ExportablePort>> RecordExport;

        public PortExporter(IShippingRecorderFactory factory)
            => _factory = factory;

        /// <summary>
        /// Export the ports to a CSV file
        /// </summary>
        /// <param name="file"></param>
        public async Task ExportAsync(string file)
        {
            var ports = await _factory.Ports.ListAsync(x => true, 1, int.MaxValue).ToListAsync();
            await ExportAsync(ports, file);
        }

        /// <summary>
        /// Export a collection of ports to a CSV file
        /// </summary>
        /// <param name="ports"></param>
        /// <param name="file"></param>
#pragma warning disable CS1998
        public async Task ExportAsync(IEnumerable<Port> ports, string file)
        {
            // Convert the ports to exportable (flattened hierarchy) ports
            var exportable = ports.ToExportable();

            // Configure an exporter to export them
            var exporter = new CsvExporter<ExportablePort>(ExportableEntityBase.DateFormat);
            exporter.RecordExport += OnRecordExported;

            // Export the records
            exporter.Export(exportable, file, ',');
        }
#pragma warning restore CS1998

        /// <summary>
        /// Handler for port export notifications
        /// </summary>
        /// <param name="_"></param>
        /// <param name="e"></param>
        private void OnRecordExported(object _, ExportEventArgs<ExportablePort> e)
        {
            RecordExport?.Invoke(this, e);
        }
    }
}
