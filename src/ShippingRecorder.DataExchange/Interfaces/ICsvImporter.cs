using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ShippingRecorder.DataExchange.Entities;

namespace ShippingRecorder.DataExchange.Interfaces
{
    public interface ICsvImporter<T> where T : class
    {
        event EventHandler<ImportEventArgs<T>> RecordImport;
        Task ImportAsync(IEnumerable<string> records);
        Task ImportAsync(string file);
    }
}