using Microsoft.AspNetCore.Mvc.Rendering;
using ShippingRecorder.Entities.Db;

namespace ShippingRecorder.Mvc.Models
{
    public class VoyageModel : Voyage
    {
        public string Message { get; set; }
        public IList<SelectListItem> Operators { get; set; } = [];
        public IList<SelectListItem> Vessels { get; set; } = [];

        public void Clear()
        {
            Id = 0;
            Number = "";
            OperatorId = 0;
            VesselId = 0;
            Message = "";
        }
    }
}