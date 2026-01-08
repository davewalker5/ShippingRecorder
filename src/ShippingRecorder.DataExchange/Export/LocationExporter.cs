using System.Linq;
using System.Threading.Tasks;
using ShippingRecorder.DataExchange.Entities;
using ShippingRecorder.DataExchange.Interfaces;
using ShippingRecorder.Entities.Db;
using ShippingRecorder.Entities.Interfaces;

namespace ShippingRecorder.DataExchange.Export
{
    public class LocationExporter : NamedEntityExporter<ExportableLocation, Location>, ILocationExporter
    {
        public LocationExporter(IShippingRecorderFactory factory) : base(factory)
        {
        }

        /// <summary>
        /// Export the locations to a CSV file
        /// </summary>
        /// <param name="file"></param>
        public async Task ExportAsync(string file)
        {
            var vessels = await _factory.Locations.ListAsync(x => true, 1, int.MaxValue).ToListAsync();
            await ExportAsync(vessels, file);
        }
    }
}