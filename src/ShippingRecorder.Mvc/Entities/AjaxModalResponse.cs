using ShippingRecorder.Entities.Db;

namespace HealthTracker.Mvc.Models
{
    public class AjaxModalResponse : ShippingRecorderEntityBase
    {
        public string Title { get; set; }
        public string HtmlContent { get; set; }
    }
}
