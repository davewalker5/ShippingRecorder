using System;
using System.Diagnostics.CodeAnalysis;

namespace ShippingRecorder.Entities.Exceptions
{
    [ExcludeFromCodeCoverage]
    public class VesselTypeNotFoundException : Exception
    {
        public VesselTypeNotFoundException()
        {
        }

        public VesselTypeNotFoundException(string message) : base(message)
        {
        }

        public VesselTypeNotFoundException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}
