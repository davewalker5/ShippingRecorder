using System;
using System.Diagnostics.CodeAnalysis;

namespace ShippingRecorder.Entities.Exceptions
{
    [ExcludeFromCodeCoverage]
    public class InvalidCountryCodeException : Exception
    {
        public InvalidCountryCodeException()
        {
        }

        public InvalidCountryCodeException(string message) : base(message)
        {
        }

        public InvalidCountryCodeException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}
