using System;
using System.Diagnostics.CodeAnalysis;

namespace ShippingRecorder.Entities.Exceptions
{
    [ExcludeFromCodeCoverage]
    public class InvalidYearBuiltException : Exception
    {
        public InvalidYearBuiltException()
        {
        }

        public InvalidYearBuiltException(string message) : base(message)
        {
        }

        public InvalidYearBuiltException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}
