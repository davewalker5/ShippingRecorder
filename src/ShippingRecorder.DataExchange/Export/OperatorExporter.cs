using System.Linq;
using System.Threading.Tasks;
using ShippingRecorder.DataExchange.Entities;
using ShippingRecorder.DataExchange.Interfaces;
using ShippingRecorder.Entities.Db;
using ShippingRecorder.Entities.Interfaces;

namespace ShippingRecorder.DataExchange.Export
{
    public class OperatorExporter : NamedEntityExporter<ExportableOperator, Operator>, IOperatorExporter
    {
        public OperatorExporter(IShippingRecorderFactory factory) : base(factory)
        {
        }

        /// <summary>
        /// Export the operators to a CSV file
        /// </summary>
        /// <param name="file"></param>
        public async Task ExportAsync(string file)
        {
            var vessels = await _factory.Operators.ListAsync(x => true, 1, int.MaxValue).ToListAsync();
            await ExportAsync(vessels, file);
        }
    }
}