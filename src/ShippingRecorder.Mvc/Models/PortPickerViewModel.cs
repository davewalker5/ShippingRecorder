using Microsoft.AspNetCore.Mvc.Rendering;
using ShippingRecorder.Entities.Db;

namespace ShippingRecorder.Mvc.Models
{
    public class PortPickerViewModel : ShippingRecorderEntityBase
    {
        public IList<SelectListItem> Countries { get; set; }
        public string DestinationControl { get; set; }
    }
}