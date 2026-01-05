using Microsoft.AspNetCore.Mvc.Rendering;
using ShippingRecorder.Entities.Db;

namespace ShippingRecorder.Mvc.Models
{
    public class VesselModel
    {
        public Vessel Vessel { get; set; } = new();
        public RegistrationHistory Registration { get; set; } = new() { Date = DateTime.Today };
        public IList<SelectListItem> Countries { get; set; } = [];
        public IList<SelectListItem> Operators { get; set; } = [];
        public IList<SelectListItem> VesselTypes { get; set; } = [];
        
    }
}