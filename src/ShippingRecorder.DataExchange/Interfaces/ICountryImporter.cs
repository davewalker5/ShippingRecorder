using ShippingRecorder.DataExchange.Entities;

namespace ShippingRecorder.DataExchange.Interfaces
{
    public interface ICountryImporter : ICsvImporter<ExportableCountry>
    {
    }
}
