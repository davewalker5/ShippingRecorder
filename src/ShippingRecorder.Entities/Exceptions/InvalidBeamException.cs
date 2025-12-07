using System;
using System.Diagnostics.CodeAnalysis;

namespace ShippingRecorder.Entities.Exceptions
{
    [ExcludeFromCodeCoverage]
    public class InvalidBeamException : Exception
    {
        public InvalidBeamException()
        {
        }

        public InvalidBeamException(string message) : base(message)
        {
        }

        public InvalidBeamException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}
