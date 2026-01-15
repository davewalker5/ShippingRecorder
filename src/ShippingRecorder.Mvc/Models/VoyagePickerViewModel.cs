using Microsoft.AspNetCore.Mvc.Rendering;
using ShippingRecorder.Entities.Db;

namespace ShippingRecorder.Mvc.Models
{
    public class VoyagePickerViewModel : ShippingRecorderEntityBase
    {
        public IList<SelectListItem> Operators { get; set; }
        public string DestinationIdControl { get; set; }
        public string DestinationNameControl { get; set; }
    }
}