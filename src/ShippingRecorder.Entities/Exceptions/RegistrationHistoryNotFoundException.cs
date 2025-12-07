using System;
using System.Diagnostics.CodeAnalysis;

namespace ShippingRecorder.Entities.Exceptions
{
    [ExcludeFromCodeCoverage]
    public class RegistrationHistoryNotFoundException : Exception
    {
        public RegistrationHistoryNotFoundException()
        {
        }

        public RegistrationHistoryNotFoundException(string message) : base(message)
        {
        }

        public RegistrationHistoryNotFoundException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}
