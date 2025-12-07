using System;
using System.Diagnostics.CodeAnalysis;

namespace ShippingRecorder.Entities.Exceptions
{
    [ExcludeFromCodeCoverage]
    public class PortExistsException : Exception
    {
        public PortExistsException()
        {
        }

        public PortExistsException(string message) : base(message)
        {
        }

        public PortExistsException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}
