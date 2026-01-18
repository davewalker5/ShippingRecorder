using System;
using System.Diagnostics.CodeAnalysis;

namespace ShippingRecorder.Entities.Exceptions
{
    [ExcludeFromCodeCoverage]
    public class InvalidVesselIdentifierException : Exception
    {
        public InvalidVesselIdentifierException()
        {
        }

        public InvalidVesselIdentifierException(string message) : base(message)
        {
        }

        public InvalidVesselIdentifierException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}
