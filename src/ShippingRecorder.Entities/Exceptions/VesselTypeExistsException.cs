using System;
using System.Diagnostics.CodeAnalysis;

namespace ShippingRecorder.Entities.Exceptions
{
    [ExcludeFromCodeCoverage]
    public class VesselTypeExistsException : Exception
    {
        public VesselTypeExistsException()
        {
        }

        public VesselTypeExistsException(string message) : base(message)
        {
        }

        public VesselTypeExistsException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}
