using System;
using System.Diagnostics.CodeAnalysis;

namespace ShippingRecorder.Entities.Exceptions
{
    [ExcludeFromCodeCoverage]
    public class VoyageNotFoundException : Exception
    {
        public VoyageNotFoundException()
        {
        }

        public VoyageNotFoundException(string message) : base(message)
        {
        }

        public VoyageNotFoundException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}
