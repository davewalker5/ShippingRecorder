using ShippingRecorder.Entities.Db;

namespace ShippingRecorder.Mvc.Models
{
    public class AddCountryViewModel : Country
    {
        public string Message { get; set; }

        public void Clear()
        {
            Id = 0;
            Code = "";
            Name = "";
            Message = "";
        }
    }
}

