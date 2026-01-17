using Microsoft.AspNetCore.Mvc.Rendering;
using ShippingRecorder.Entities.Db;

namespace ShippingRecorder.Mvc.Models
{
    public class PortModel : Port
    {
        public IList<SelectListItem> Countries { get; set; }
        public string Message { get; set; }

        public void Clear()
        {
            Id = 0;
            CountryId = 0;
            Code = "";
            Name = "";
            Message = "";
        }
    }
}