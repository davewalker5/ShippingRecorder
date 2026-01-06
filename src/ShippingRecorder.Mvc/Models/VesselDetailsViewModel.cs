using ShippingRecorder.Entities.Db;

namespace ShippingRecorder.Mvc.Models
{
    public class VesselDetailsViewModel : VesselModel
    {
        public Sighting MostRecentSighting { get; set; }

        public string Action { get; set; }
    }
}
