using System;
using System.Diagnostics.CodeAnalysis;

namespace ShippingRecorder.Entities.Exceptions
{
    [ExcludeFromCodeCoverage]
    public class VesselExistsException : Exception
    {
        public VesselExistsException()
        {
        }

        public VesselExistsException(string message) : base(message)
        {
        }

        public VesselExistsException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}
