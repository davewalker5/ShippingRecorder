using ShippingRecorder.Entities.Db;

namespace ShippingRecorder.Mvc.Models
{
    public class SightingSummaryViewModel
    {
        public string Message { get; set; }
        public Sighting Sighting { get; set; }
    }
}
