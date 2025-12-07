using ShippingRecorder.DataExchange.Entities;

namespace ShippingRecorder.DataExchange.Interfaces
{
    public interface ILocationImporter : ICsvImporter<ExportableLocation>
    {
    }
}
