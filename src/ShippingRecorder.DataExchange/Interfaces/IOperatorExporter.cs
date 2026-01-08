using ShippingRecorder.DataExchange.Entities;
using ShippingRecorder.Entities.Db;

namespace ShippingRecorder.DataExchange.Interfaces
{
    public interface IOperatorExporter :
        INamedEntityExporter<ExportableOperator, Operator>,
        IExporter<ExportableOperator, Operator>
    {
    }
}