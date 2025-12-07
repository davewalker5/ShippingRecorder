using System;
using System.Diagnostics.CodeAnalysis;

namespace ShippingRecorder.Entities.Exceptions
{
    [ExcludeFromCodeCoverage]
    public class InvalidLengthException : Exception
    {
        public InvalidLengthException()
        {
        }

        public InvalidLengthException(string message) : base(message)
        {
        }

        public InvalidLengthException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}
