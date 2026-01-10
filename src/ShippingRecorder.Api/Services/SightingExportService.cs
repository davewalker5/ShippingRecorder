using ShippingRecorder.Api.Entities;
using ShippingRecorder.Api.Interfaces;
using ShippingRecorder.DataExchange.Export;
using Microsoft.Extensions.Options;
using ShippingRecorder.Entities.Interfaces;
using ShippingRecorder.Entities.Config;

namespace ShippingRecorder.Api.Services
{
    public class SightingExportService : BackgroundQueueProcessor<SightingExportWorkItem>
    {
        private readonly ShippingRecorderApplicationSettings _settings;

        public SightingExportService(
            ILogger<BackgroundQueueProcessor<SightingExportWorkItem>> logger,
            IBackgroundQueue<SightingExportWorkItem> queue,
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
        protected override async Task ProcessWorkItemAsync(SightingExportWorkItem item, IShippingRecorderFactory factory)
        {
            // Get the list of items to export
            MessageLogger.LogInformation("Retrieving sightings for export");
            var sightings = await factory.Sightings.ListAsync(x => true, 1, int.MaxValue).ToListAsync();

            // Get the full path to the export file
            var filePath = Path.Combine(_settings.ExportPath, item.FileName);

            // Export the items
            MessageLogger.LogInformation($"Exporting {sightings.Count} sightings to {filePath}");
            var exporter = new SightingExporter(factory);
            await exporter.ExportAsync(sightings, filePath);
            MessageLogger.LogInformation("Sighting export completed");
        }
    }
}