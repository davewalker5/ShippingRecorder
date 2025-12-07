using System;
using System.Diagnostics.CodeAnalysis;

namespace ShippingRecorder.Entities.Exceptions
{
    [ExcludeFromCodeCoverage]
    public class VoyageExistsException : Exception
    {
        public VoyageExistsException()
        {
        }

        public VoyageExistsException(string message) : base(message)
        {
        }

        public VoyageExistsException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}
