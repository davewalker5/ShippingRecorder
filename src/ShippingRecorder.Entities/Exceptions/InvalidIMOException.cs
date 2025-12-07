using System;
using System.Diagnostics.CodeAnalysis;

namespace ShippingRecorder.Entities.Exceptions
{
    [ExcludeFromCodeCoverage]
    public class InvalidIMOException : Exception
    {
        public InvalidIMOException()
        {
        }

        public InvalidIMOException(string message) : base(message)
        {
        }

        public InvalidIMOException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}
