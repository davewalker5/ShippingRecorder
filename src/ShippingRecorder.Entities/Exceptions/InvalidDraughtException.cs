using System;
using System.Diagnostics.CodeAnalysis;

namespace ShippingRecorder.Entities.Exceptions
{
    [ExcludeFromCodeCoverage]
    public class InvalidDraughtException : Exception
    {
        public InvalidDraughtException()
        {
        }

        public InvalidDraughtException(string message) : base(message)
        {
        }

        public InvalidDraughtException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}
