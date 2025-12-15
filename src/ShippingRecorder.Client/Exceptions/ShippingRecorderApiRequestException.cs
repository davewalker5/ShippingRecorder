using System;
using System.Diagnostics.CodeAnalysis;

namespace ShippingRecorder.Client.Exceptions
{
    [ExcludeFromCodeCoverage]
    public class ShippingRecorderApiRequestException : Exception
    {
        public ShippingRecorderApiRequestException()
        {
        }

        public ShippingRecorderApiRequestException(string message) : base(message)
        {
        }

        public ShippingRecorderApiRequestException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}
