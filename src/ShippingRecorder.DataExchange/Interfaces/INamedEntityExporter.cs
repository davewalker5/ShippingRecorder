using ShippingRecorder.DataExchange.Entities;
using ShippingRecorder.Entities.Db;
using ShippingRecorder.Entities.Interfaces;

namespace ShippingRecorder.DataExchange.Interfaces
{
    public interface INamedEntityExporter<E, D>
        where E : ExportableEntityBase, INamedEntity
        where D : ShippingRecorderEntityBase, INamedEntity
    {
    }
}