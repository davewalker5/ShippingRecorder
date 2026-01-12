using Microsoft.AspNetCore.Mvc.Rendering;
using ShippingRecorder.Entities.Db;

namespace ShippingRecorder.Mvc.Models
{
    public class VoyageModel : Voyage
    {
        public bool Editable { get; set; } = true;
        public IList<SelectListItem> Operators { get; set; } = [];
        public IList<SelectListItem> Vessels { get; set; } = [];
    }
}