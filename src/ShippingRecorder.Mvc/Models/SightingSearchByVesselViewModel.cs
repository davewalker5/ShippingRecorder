using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ShippingRecorder.Mvc.Models
{
    public class SightingSearchByVesselViewModel : SightingSearchViewModelBase
    {
        [DisplayName("IMO")]
        [StringLength(7, MinimumLength = 7, ErrorMessage = "IMO must be 7 digits long")]
        [RegularExpression(@"^\d+$", ErrorMessage = "IMO must contain digits only")]
        [Required(ErrorMessage = "You must provide a vessel IMO")]
        public string IMO { get; set; }
    }
}
