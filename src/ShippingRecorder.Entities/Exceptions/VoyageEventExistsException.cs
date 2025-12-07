using System;
using System.Diagnostics.CodeAnalysis;

namespace ShippingRecorder.Entities.Exceptions
{
    [ExcludeFromCodeCoverage]
    public class VoyageEventExistsException : Exception
    {
        public VoyageEventExistsException()
        {
        }

        public VoyageEventExistsException(string message) : base(message)
        {
        }

        public VoyageEventExistsException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}
