using ShippingRecorder.DataExchange.Entities;
using ShippingRecorder.Entities.Db;

namespace ShippingRecorder.DataExchange.Interfaces
{
    public interface IPortExporter : IExporter<ExportablePort, Port>
    {
    }
}