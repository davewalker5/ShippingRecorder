using System.Collections.Generic;
using ShippingRecorder.Config.Entities;
using ShippingRecorder.Entities.Interfaces;
using ShippingRecorder.Entities.Logging;

namespace ShippingRecorder.Entities.Config
{
    public class ShippingRecorderApplicationSettings : IShippingRecorderApplicationSettings
    {
        public string ApiUrl { get; set; }
        public string Secret { get; set; }
        public int TokenLifespanMinutes { get; set; }
        public List<ApiRoute> ApiRoutes { get; set; }
        public string LogFile { get; set; }
        public Severity MinimumLogLevel { get; set; }
        public int DefaultTimePeriodDays { get; set; }
        public bool UseCustomErrorPageInDevelopment { get; set; }
    }
}