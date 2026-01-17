using ShippingRecorder.Api.Entities;
using ShippingRecorder.Api.Interfaces;
using ShippingRecorder.DataExchange.Export;
using Microsoft.Extensions.Options;
using ShippingRecorder.Entities.Interfaces;
using ShippingRecorder.Entities.Config;

namespace ShippingRecorder.Api.Services
{
    public class PortExportService : BackgroundQueueProcessor<PortExportWorkItem>
    {
        private readonly ShippingRecorderApplicationSettings _settings;

        public PortExportService(
            ILogger<BackgroundQueueProcessor<PortExportWorkItem>> logger,
            IBackgroundQueue<PortExportWorkItem> queue,
            IServiceScopeFactory serviceScopeFactory,
            IOptions<ShippingRecorderApplicationSettings> settings)
            : base(logger, queue, serviceScopeFactory)
        {
            _settings = settings.Value;
        }

        /// <summary>
        /// Export the data
        /// </summary>
        /// <param name="item"></param>
        /// <param name="factory"></param>
        /// <returns></returns>
        protected override async Task ProcessWorkItemAsync(PortExportWorkItem item, IShippingRecorderFactory factory)
        {
            // Get the list of items to export
            MessageLogger.LogInformation("Retrieving ports for export");
            var ports = await factory.Ports.ListAsync(x => true, 1, int.MaxValue).ToListAsync();

            // Get the full path to the export file
            var filePath = Path.Combine(_settings.ExportPath, item.FileName);

            // Export the items
            MessageLogger.LogInformation($"Exporting {ports.Count} ports to {filePath}");
            var exporter = new PortExporter(factory);
            await exporter.ExportAsync(ports, filePath);
            MessageLogger.LogInformation("Port export completed");
        }
    }
}