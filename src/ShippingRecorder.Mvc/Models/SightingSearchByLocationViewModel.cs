using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ShippingRecorder.Mvc.Models
{
    public class SightingSearchByLocationViewModel : SightingSearchViewModelBase
    {
        [DisplayName("Location")]
        [Required(ErrorMessage = "You must select a location")]
        public int? LocationId { get; set; }

        public IList<SelectListItem> Locations { get; set; }
    }
}
