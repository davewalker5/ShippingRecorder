using System;
using System.Diagnostics.CodeAnalysis;

namespace ShippingRecorder.Entities.Exceptions
{
    [ExcludeFromCodeCoverage]
    public class InvalidMMSIException : Exception
    {
        public InvalidMMSIException()
        {
        }

        public InvalidMMSIException(string message) : base(message)
        {
        }

        public InvalidMMSIException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}
