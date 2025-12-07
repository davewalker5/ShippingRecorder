using ShippingRecorder.Entities.Interfaces;
using ShippingRecorder.Entities.Logging;
using System;
using System.Diagnostics;

namespace ShippingRecorder.Tests.Mocks
{
    public class MockFileLogger : IShippingRecorderLogger
    {
        public void Initialise(string logFile, Severity minimumSeverityToLog)
        {
        }

        public void LogMessage(Severity severity, string message)
        {
            Debug.Print($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} [{severity.ToString()}] {message}");
        }

        public void LogException(Exception ex)
        {
            LogMessage(Severity.Error, ex.Message);
            LogMessage(Severity.Error, ex.ToString());
        }
    }
}
