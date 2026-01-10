using ShippingRecorder.Api.Entities;
using ShippingRecorder.Api.Interfaces;
using ShippingRecorder.DataExchange.Export;
using Microsoft.Extensions.Options;
using ShippingRecorder.Entities.Interfaces;
using ShippingRecorder.Entities.Config;

namespace ShippingRecorder.Api.Services
{
    public class LocationExportService : BackgroundQueueProcessor<LocationExportWorkItem>
    {
        private readonly ShippingRecorderApplicationSettings _settings;

        public LocationExportService(
            ILogger<BackgroundQueueProcessor<LocationExportWorkItem>> logger,
            IBackgroundQueue<LocationExportWorkItem> queue,
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
        protected override async Task ProcessWorkItemAsync(LocationExportWorkItem item, IShippingRecorderFactory factory)
        {
            // Get the list of items to export
            MessageLogger.LogInformation("Retrieving locations for export");
            var locations = await factory.Locations.ListAsync(x => true, 1, int.MaxValue).ToListAsync();

            // Get the full path to the export file
            var filePath = Path.Combine(_settings.ExportPath, item.FileName);

            // Export the items
            MessageLogger.LogInformation($"Exporting {locations.Count} locations to {filePath}");
            var exporter = new LocationExporter(factory);
            await exporter.ExportAsync(locations, filePath);
            MessageLogger.LogInformation("Location export completed");
        }
    }
}