using ShippingRecorder.DataExchange.Entities;
using ShippingRecorder.Entities.Db;

namespace ShippingRecorder.DataExchange.Interfaces
{
    public interface ISightingExporter : IExporter<ExportableSighting, Sighting>
    {
    }
}