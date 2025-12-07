using System;
using System.Diagnostics.CodeAnalysis;

namespace ShippingRecorder.DataExchange.Exceptions
{
    [ExcludeFromCodeCoverage]
    public class InvalidFieldValueException : Exception
    {
        public InvalidFieldValueException()
        {
        }

        public InvalidFieldValueException(string message) : base(message)
        {
        }

        public InvalidFieldValueException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}
