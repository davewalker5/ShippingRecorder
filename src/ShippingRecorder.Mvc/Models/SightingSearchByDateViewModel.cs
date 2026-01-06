using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ShippingRecorder.Mvc.Models
{
    public class SightingSearchByDateViewModel : SightingSearchViewModelBase
    {
        [DisplayName("From")]
        [UIHint("DatePicker")]
        [DataType(DataType.Date)]
        [Required(ErrorMessage = "You must enter a 'from' date")]
        public DateTime? From { get; set; }

        [DisplayName("To")]
        [UIHint("DatePicker")]
        [DataType(DataType.Date)]
        [Required(ErrorMessage = "You must enter a 'to' date")]
        public DateTime? To { get; set; }

    }
}
