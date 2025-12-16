using ShippingRecorder.Entities.Db;

namespace ShippingRecorder.Mvc.Models
{
    public class AddLocationViewModel : Location
    {
        public string Message { get; set; }

        public void Clear()
        {
            Id = 0;
            Name = "";
            Message = "";
        }
    }
}