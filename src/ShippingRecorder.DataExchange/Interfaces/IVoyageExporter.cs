using ShippingRecorder.DataExchange.Entities;
using ShippingRecorder.Entities.Db;

namespace ShippingRecorder.DataExchange.Interfaces
{
    public interface IVoyageExporter : IExporter<ExportableVoyage, Voyage>
    {
    }
}