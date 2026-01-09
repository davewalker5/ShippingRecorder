using ShippingRecorder.Api.Entities;
using ShippingRecorder.Api.Interfaces;
using ShippingRecorder.DataExchange.Import;
using ShippingRecorder.Entities.Interfaces;
using ShippingRecorder.DataExchange.Entities;

namespace ShippingRecorder.Api.Services
{
    public class PortImportService : BackgroundQueueProcessor<PortImportWorkItem>
    {
        public PortImportService(
            ILogger<BackgroundQueueProcessor<PortImportWorkItem>> logger,
            IBackgroundQueue<PortImportWorkItem> queue,
            IServiceScopeFactory serviceScopeFactory)
            : base(logger, queue, serviceScopeFactory)
        {
        }

        /// <summary>
        /// Import from the data specified in the work item
        /// </summary>
        /// <param name="item"></param>
        /// <param name="factory"></param>
        /// <returns></returns>
        protected override async Task ProcessWorkItemAsync(PortImportWorkItem item, IShippingRecorderFactory factory)
        {
            MessageLogger.LogInformation("Port import started");
            var records = item.Content.Split('\n', StringSplitOptions.RemoveEmptyEntries);

            var count = records.Length - 1;
            if (count > 0)
            {
                var messageEnding = (count > 1) ? "s" : "";
                MessageLogger.LogInformation($"Importing {records.Count() - 1} port{messageEnding}");
                var importer = new PortImporter(factory, ExportablePort.CsvRecordPattern);
                await importer.ImportAsync(records);
            }
            else
            {
                MessageLogger.LogWarning("No records found to import");
            }

            MessageLogger.LogInformation("Port import completed");
        }
    }
}