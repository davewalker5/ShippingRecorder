using ShippingRecorder.Api.Entities;
using ShippingRecorder.Api.Interfaces;
using ShippingRecorder.DataExchange.Export;
using Microsoft.Extensions.Options;
using ShippingRecorder.Entities.Interfaces;
using ShippingRecorder.Entities.Config;

namespace ShippingRecorder.Api.Services
{
    public class VesselExportService : BackgroundQueueProcessor<VesselExportWorkItem>
    {
        private readonly ShippingRecorderApplicationSettings _settings;

        public VesselExportService(
            ILogger<BackgroundQueueProcessor<VesselExportWorkItem>> logger,
            IBackgroundQueue<VesselExportWorkItem> queue,
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
        protected override async Task ProcessWorkItemAsync(VesselExportWorkItem item, IShippingRecorderFactory factory)
        {
            // Get the list of items to export
            MessageLogger.LogInformation("Retrieving vessels for export");
            var vessels = await factory.Vessels.ListAsync(x => true, 1, int.MaxValue).ToListAsync();

            // Get the full path to the export file
            var filePath = Path.Combine(_settings.ExportPath, item.FileName);

            // Export the items
            MessageLogger.LogInformation($"Exporting {vessels.Count} vessels to {filePath}");
            var exporter = new VesselExporter(factory);
            await exporter.ExportAsync(vessels, filePath);
            MessageLogger.LogInformation("Vessel export completed");
        }
    }
}