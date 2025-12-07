using System;
using System.Diagnostics.CodeAnalysis;

namespace ShippingRecorder.DataExchange.Entities
{
    [ExcludeFromCodeCoverage]
    public class ImportEventArgs<T> : EventArgs where T : class
    {
        public long RecordCount { get; set; }
        public T Entity { get; set; }
    }
}
