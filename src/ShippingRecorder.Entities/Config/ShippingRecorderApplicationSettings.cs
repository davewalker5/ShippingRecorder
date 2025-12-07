using ShippingRecorder.Entities.Logging;

namespace ShippingRecorder.Entities.Config
{
    public class ShippingRecorderApplicationSettings
    {
        public string Secret { get; set; }
        public int TokenLifespanMinutes { get; set; }
        public string LogFile { get; set; }
        public Severity MinimumLogLevel { get; set; }
    }
}