using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ShippingRecorder.DataExchange.Entities;
using ShippingRecorder.Entities.Db;

namespace ShippingRecorder.DataExchange.Interfaces
{
    public interface IExporter<E, D> where E : ExportableEntityBase where D : ShippingRecorderEntityBase
    {
        event EventHandler<ExportEventArgs<E>> RecordExport;
        Task ExportAsync(string file);
        Task ExportAsync(IEnumerable<D> sightings, string file);
    }
}