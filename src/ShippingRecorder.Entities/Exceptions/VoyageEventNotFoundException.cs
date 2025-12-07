using System;
using System.Diagnostics.CodeAnalysis;

namespace ShippingRecorder.Entities.Exceptions
{
    [ExcludeFromCodeCoverage]
    public class VoyageEventNotFoundException : Exception
    {
        public VoyageEventNotFoundException()
        {
        }

        public VoyageEventNotFoundException(string message) : base(message)
        {
        }

        public VoyageEventNotFoundException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}
