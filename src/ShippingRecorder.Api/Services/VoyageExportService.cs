using ShippingRecorder.Api.Entities;
using ShippingRecorder.Api.Interfaces;
using ShippingRecorder.DataExchange.Export;
using Microsoft.Extensions.Options;
using ShippingRecorder.Entities.Interfaces;
using ShippingRecorder.Entities.Config;

namespace ShippingRecorder.Api.Services
{
    public class VoyageExportService : BackgroundQueueProcessor<VoyageExportWorkItem>
    {
        private readonly ShippingRecorderApplicationSettings _settings;

        public VoyageExportService(
            ILogger<BackgroundQueueProcessor<VoyageExportWorkItem>> logger,
            IBackgroundQueue<VoyageExportWorkItem> queue,
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
        protected override async Task ProcessWorkItemAsync(VoyageExportWorkItem item, IShippingRecorderFactory factory)
        {
            // Get the list of items to export
            MessageLogger.LogInformation("Retrieving voyages for export");
            var voyages = await factory.Voyages.ListAsync(x => true, 1, int.MaxValue).ToListAsync();

            // Get the full path to the export file
            var filePath = Path.Combine(_settings.ExportPath, item.FileName);

            // Export the items
            MessageLogger.LogInformation($"Exporting {voyages.Count} voyages to {filePath}");
            var exporter = new VoyageExporter(factory);
            await exporter.ExportAsync(voyages, filePath);
            MessageLogger.LogInformation("Voyage export completed");
        }
    }
}