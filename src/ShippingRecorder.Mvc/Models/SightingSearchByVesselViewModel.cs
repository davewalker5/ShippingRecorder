using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ShippingRecorder.Mvc.Models
{
    public class SightingSearchByVesselViewModel : SightingSearchViewModelBase
    {
        [DisplayName("Vessel Identifier")]
        [StringLength(7, MinimumLength = 7, ErrorMessage = "Vessel identifier must be 7 digits long")]
        [RegularExpression(@"^\d+$", ErrorMessage = "Vessel identifier must contain digits only")]
        [Required(ErrorMessage = "You must provide a vessel identifier")]
        public string Identifier { get; set; }
    }
}
