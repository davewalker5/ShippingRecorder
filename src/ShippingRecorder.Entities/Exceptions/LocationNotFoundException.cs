using System;
using System.Diagnostics.CodeAnalysis;

namespace ShippingRecorder.Entities.Exceptions
{
    [ExcludeFromCodeCoverage]
    public class LocationNotFoundException : Exception
    {
        public LocationNotFoundException()
        {
        }

        public LocationNotFoundException(string message) : base(message)
        {
        }

        public LocationNotFoundException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}
