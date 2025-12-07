using ShippingRecorder.DataExchange.Entities;

namespace ShippingRecorder.DataExchange.Interfaces
{
    public interface IVoyageImporter : ICsvImporter<ExportableVoyage>
    {
    }
}
