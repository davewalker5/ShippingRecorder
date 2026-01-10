using ShippingRecorder.Api.Entities;
using ShippingRecorder.Api.Interfaces;
using ShippingRecorder.DataExchange.Export;
using Microsoft.Extensions.Options;
using ShippingRecorder.Entities.Interfaces;
using ShippingRecorder.Entities.Config;

namespace ShippingRecorder.Api.Services
{
    public class VesselTypeExportService : BackgroundQueueProcessor<VesselTypeExportWorkItem>
    {
        private readonly ShippingRecorderApplicationSettings _settings;

        public VesselTypeExportService(
            ILogger<BackgroundQueueProcessor<VesselTypeExportWorkItem>> logger,
            IBackgroundQueue<VesselTypeExportWorkItem> queue,
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
        protected override async Task ProcessWorkItemAsync(VesselTypeExportWorkItem item, IShippingRecorderFactory factory)
        {
            // Get the list of items to export
            MessageLogger.LogInformation("Retrieving vessel types for export");
            var vesselTypes = await factory.VesselTypes.ListAsync(x => true, 1, int.MaxValue).ToListAsync();

            // Get the full path to the export file
            var filePath = Path.Combine(_settings.ExportPath, item.FileName);

            // Export the items
            MessageLogger.LogInformation($"Exporting {vesselTypes.Count} vessel types to {filePath}");
            var exporter = new VesselTypeExporter(factory);
            await exporter.ExportAsync(vesselTypes, filePath);
            MessageLogger.LogInformation("Vessel type export completed");
        }
    }
}