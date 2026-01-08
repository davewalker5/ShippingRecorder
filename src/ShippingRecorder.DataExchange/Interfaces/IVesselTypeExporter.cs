using ShippingRecorder.DataExchange.Entities;
using ShippingRecorder.Entities.Db;

namespace ShippingRecorder.DataExchange.Interfaces
{
    public interface IVesselTypeExporter :
        INamedEntityExporter<ExportableVesselType, VesselType>,
        IExporter<ExportableVesselType, VesselType>
    {
    }
}