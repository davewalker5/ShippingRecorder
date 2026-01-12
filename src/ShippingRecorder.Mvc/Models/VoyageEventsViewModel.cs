using Microsoft.AspNetCore.Mvc.Rendering;
using ShippingRecorder.Entities.Db;

namespace ShippingRecorder.Mvc.Models
{
    public class VoyageEventsViewModel : ShippingRecorderEntityBase
    {
        public bool Editable { get; set; } = true;
        public Voyage Voyage { get; set; }
        public IList<SelectListItem> Operators { get; set; } = [];
        public IList<SelectListItem> Vessels { get; set; } = [];
    }
}