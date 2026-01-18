using ShippingRecorder.Entities.Db;

namespace ShippingRecorder.Mvc.Models
{
    public class AddVesselViewModel : VesselModel
    {
        public string Message { get; set; }

        public void Clear()
        {
            Vessel.Id = 0;
            Vessel.Identifier = "";
            Vessel.Built = null;
            Vessel.Draught = null;
            Vessel.Length = null;
            Vessel.Beam = null;
            Message = "";
        }
    }
}