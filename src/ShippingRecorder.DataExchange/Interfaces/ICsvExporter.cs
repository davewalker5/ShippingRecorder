using System;
using System.Collections.Generic;
using ShippingRecorder.DataExchange.Entities;

namespace ShippingRecorder.DataExchange.Interfaces
{
    public interface ICsvExporter<T> where T : class
    {
        event EventHandler<ExportEventArgs<T>> RecordExport;
        void Export(IEnumerable<T> entities, string fileName, char separator);
    }
}
