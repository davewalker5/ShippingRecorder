using ShippingRecorder.Api.Entities;
using ShippingRecorder.Api.Interfaces;
using ShippingRecorder.DataExchange.Import;
using ShippingRecorder.Entities.Interfaces;
using ShippingRecorder.DataExchange.Entities;

namespace ShippingRecorder.Api.Services
{
    public class LocationImportService : BackgroundQueueProcessor<LocationImportWorkItem>
    {
        public LocationImportService(
            ILogger<BackgroundQueueProcessor<LocationImportWorkItem>> logger,
            IBackgroundQueue<LocationImportWorkItem> queue,
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
        protected override async Task ProcessWorkItemAsync(LocationImportWorkItem item, IShippingRecorderFactory factory)
        {
            MessageLogger.LogInformation("Location import started");
            var records = item.Content.Split('\n', StringSplitOptions.RemoveEmptyEntries);

            var count = records.Length - 1;
            if (count > 0)
            {
                var messageEnding = (count > 1) ? "s" : "";
                MessageLogger.LogInformation($"Importing {records.Count() - 1} location{messageEnding}");
                var importer = new LocationImporter(factory, ExportableLocation.CsvRecordPattern);
                await importer.ImportAsync(records);
            }
            else
            {
                MessageLogger.LogWarning("No records found to import");
            }

            MessageLogger.LogInformation("Location import completed");
        }
    }
}