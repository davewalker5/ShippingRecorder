using ShippingRecorder.DataExchange.Entities;
using ShippingRecorder.Entities.Db;

namespace ShippingRecorder.DataExchange.Interfaces
{
    public interface ILocationExporter :
        INamedEntityExporter<ExportableLocation, Location>,
        IExporter<ExportableLocation, Location>
    {
    }
}