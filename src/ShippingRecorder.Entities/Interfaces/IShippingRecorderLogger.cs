using ShippingRecorder.Entities.Logging;
using System;

namespace ShippingRecorder.Entities.Interfaces
{
    public interface IShippingRecorderLogger
    {
        void Initialise(string logFile, Severity minimumSeverityToLog);
        void LogMessage(Severity severity, string message);
        void LogException(Exception ex);
    }
}
