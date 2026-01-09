using ShippingRecorder.Api.Entities;
using ShippingRecorder.Api.Interfaces;
using ShippingRecorder.DataExchange.Import;
using ShippingRecorder.Entities.Interfaces;
using ShippingRecorder.DataExchange.Entities;

namespace ShippingRecorder.Api.Services
{
    public class VesselTypeImportService : BackgroundQueueProcessor<VesselTypeImportWorkItem>
    {
        public VesselTypeImportService(
            ILogger<BackgroundQueueProcessor<VesselTypeImportWorkItem>> logger,
            IBackgroundQueue<VesselTypeImportWorkItem> queue,
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
        protected override async Task ProcessWorkItemAsync(VesselTypeImportWorkItem item, IShippingRecorderFactory factory)
        {
            MessageLogger.LogInformation("Vessel type import started");
            var records = item.Content.Split('\n', StringSplitOptions.RemoveEmptyEntries);

            var count = records.Length - 1;
            if (count > 0)
            {
                var messageEnding = (count > 1) ? "s" : "";
                MessageLogger.LogInformation($"Importing {records.Count() - 1} vessel type{messageEnding}");
                var importer = new VesselTypeImporter(factory, ExportableVesselType.CsvRecordPattern);
                await importer.ImportAsync(records);
            }
            else
            {
                MessageLogger.LogWarning("No records found to import");
            }

            MessageLogger.LogInformation("Vessel type import completed");
        }
    }
}