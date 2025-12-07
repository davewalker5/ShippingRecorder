using System;
using System.Diagnostics.CodeAnalysis;

namespace ShippingRecorder.Entities.Exceptions
{
    [ExcludeFromCodeCoverage]
    public class VesselNotFoundException : Exception
    {
        public VesselNotFoundException()
        {
        }

        public VesselNotFoundException(string message) : base(message)
        {
        }

        public VesselNotFoundException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}
