using System.ComponentModel;

namespace ShippingRecorder.Mvc.Models
{
    public class ConfirmDetailsViewModel : VesselModel
    {
        [DisplayName("Date")]
        public DateTime Date { get; set; }

        [DisplayName("Location")]
        public string Location { get; set; }

        [DisplayName("My Voyage")]
        public bool IsMyVoyage { get; set; }

        [DisplayName("My Voyage")]
        public string MyVoyageText { get { return IsMyVoyage ? "Yes" : "No"; }}

        public string Action { get; set; }
    }
}
