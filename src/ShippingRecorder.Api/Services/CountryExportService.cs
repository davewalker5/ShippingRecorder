using ShippingRecorder.Api.Entities;
using ShippingRecorder.Api.Interfaces;
using ShippingRecorder.DataExchange.Export;
using Microsoft.Extensions.Options;
using ShippingRecorder.Entities.Interfaces;
using ShippingRecorder.Entities.Config;

namespace ShippingRecorder.Api.Services
{
    public class CountryExportService : BackgroundQueueProcessor<CountryExportWorkItem>
    {
        private readonly ShippingRecorderApplicationSettings _settings;

        public CountryExportService(
            ILogger<BackgroundQueueProcessor<CountryExportWorkItem>> logger,
            IBackgroundQueue<CountryExportWorkItem> queue,
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
        protected override async Task ProcessWorkItemAsync(CountryExportWorkItem item, IShippingRecorderFactory factory)
        {
            // Get the list of items to export
            MessageLogger.LogInformation("Retrieving countries for export");
            var countries = await factory.Countries.ListAsync(x => true, 1, int.MaxValue).ToListAsync();

            // Get the full path to the export file
            var filePath = Path.Combine(_settings.ExportPath, item.FileName);

            // Export the items
            MessageLogger.LogInformation($"Exporting {countries.Count} countries to {filePath}");
            var exporter = new CountryExporter(factory);
            await exporter.ExportAsync(countries, filePath);
            MessageLogger.LogInformation("Country export completed");
        }
    }
}