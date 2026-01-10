using System.Collections.Generic;
using ShippingRecorder.Config.Entities;
using ShippingRecorder.Entities.Logging;

namespace ShippingRecorder.Entities.Interfaces
{
    public interface IShippingRecorderApplicationSettings
    {
        string ApiUrl { get; set; }
        string Secret { get; set; }
        int TokenLifespanMinutes { get; set; }
        List<ApiRoute> ApiRoutes { get; set; }
        string LogFile { get; set; }
        Severity MinimumLogLevel { get; set; }
        int DefaultTimePeriodDays { get; set; }
        bool UseCustomErrorPageInDevelopment { get; set; }
        int SearchPageSize { get; set; }
        int CacheLifetimeSeconds { get; set; }
        string DateTimeFormat { get; set; }
        string ExportPath { get; set; }
    }
}