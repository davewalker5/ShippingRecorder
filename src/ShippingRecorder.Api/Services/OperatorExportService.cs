using ShippingRecorder.Api.Entities;
using ShippingRecorder.Api.Interfaces;
using ShippingRecorder.DataExchange.Export;
using Microsoft.Extensions.Options;
using ShippingRecorder.Entities.Interfaces;
using ShippingRecorder.Entities.Config;

namespace ShippingRecorder.Api.Services
{
    public class OperatorExportService : BackgroundQueueProcessor<OperatorExportWorkItem>
    {
        private readonly ShippingRecorderApplicationSettings _settings;

        public OperatorExportService(
            ILogger<BackgroundQueueProcessor<OperatorExportWorkItem>> logger,
            IBackgroundQueue<OperatorExportWorkItem> queue,
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
        protected override async Task ProcessWorkItemAsync(OperatorExportWorkItem item, IShippingRecorderFactory factory)
        {
            // Get the list of items to export
            MessageLogger.LogInformation("Retrieving operators for export");
            var operators = await factory.Operators.ListAsync(x => true, 1, int.MaxValue).ToListAsync();

            // Get the full path to the export file
            var filePath = Path.Combine(_settings.ExportPath, item.FileName);

            // Export the items
            MessageLogger.LogInformation($"Exporting {operators.Count} operators to {filePath}");
            var exporter = new OperatorExporter(factory);
            await exporter.ExportAsync(operators, filePath);
            MessageLogger.LogInformation("Operator export completed");
        }
    }
}