using System;
using System.Diagnostics.CodeAnalysis;

namespace ShippingRecorder.Entities.Exceptions
{
    [ExcludeFromCodeCoverage]
    public class PortNotFoundException : Exception
    {
        public PortNotFoundException()
        {
        }

        public PortNotFoundException(string message) : base(message)
        {
        }

        public PortNotFoundException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}
