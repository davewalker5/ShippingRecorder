using ShippingRecorder.Entities.Db;

namespace ShippingRecorder.Mvc.Models
{
    public class AddVesselViewModel : Vessel
    {
        public string Message { get; set; }

        public void Clear()
        {
            Id = 0;
            IMO = "";
            Built = null;
            Draught = null;
            Length = null;
            Beam = null;
            Message = "";
        }
    }
}