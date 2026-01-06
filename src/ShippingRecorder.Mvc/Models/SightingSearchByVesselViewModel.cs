using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ShippingRecorder.Mvc.Models
{
    public class SightingSearchByVesselViewModel : SightingSearchViewModelBase
    {
        [DisplayName("IMO")]
        [Required(ErrorMessage = "You must provide a vessel IMO")]
        public string IMO { get; set; }
    }
}
