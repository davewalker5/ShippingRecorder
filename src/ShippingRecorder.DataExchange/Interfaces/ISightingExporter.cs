using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ShippingRecorder.DataExchange.Entities;
using ShippingRecorder.Entities.Db;

namespace ShippingRecorder.DataExchange.Interfaces
{
    public interface ISightingExporter
    {
        event EventHandler<ExportEventArgs<ExportableSighting>> RecordExport;
        Task ExportAsync(string file);
        Task ExportAsync(IEnumerable<Sighting> sightings, string file);
    }
}