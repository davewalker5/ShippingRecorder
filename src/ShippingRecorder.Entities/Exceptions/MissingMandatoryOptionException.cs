using System;
using System.Diagnostics.CodeAnalysis;

namespace ShippingRecorder.Entities.Exceptions
{
    [ExcludeFromCodeCoverage]
    public class MissingMandatoryOptionException : Exception
    {
        public MissingMandatoryOptionException()
        {
        }

        public MissingMandatoryOptionException(string message) : base(message)
        {
        }

        public MissingMandatoryOptionException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}