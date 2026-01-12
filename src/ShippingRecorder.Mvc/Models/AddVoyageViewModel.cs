using ShippingRecorder.Entities.Db;

namespace ShippingRecorder.Mvc.Models
{
    public class AddVoyageViewModel : VoyageModel
    {
        public string Message { get; set; }

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