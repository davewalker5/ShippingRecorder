using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using ShippingRecorder.Entities.Attributes;
using ShippingRecorder.Entities.Db;
using ShippingRecorder.Enumerations.Extensions;

namespace ShippingRecorder.Mvc.Models
{
    public class VoyageEventViewModel : ShippingRecorderEntityBase
    {
        public long Id { get; set; }
        public long VoyageId { get; set; }

        [DisplayName("Voyage Number")]
        public string VoyageNumber { get; set; }

        [DisplayName("Date")]
        [UIHint("DatePicker")]
        [Required(ErrorMessage = "You must provide an event date")]
        public DateTime Date { get; set; } = DateTime.Today;

        [DisplayName("Port UN/LOCODE")]
        [UNLOCODE]
        [Required(ErrorMessage = "You must provide a port UN/LOCODE")]
        public string Port { get; set; }

        [DisplayName("Event Type")]
        [Required(ErrorMessage = "You must provide an event type")]
        public VoyageEventType EventType { get; set; }
        
        public IList<SelectListItem> EventTypes { get; set; } = [];

        public VoyageEventViewModel()
        {
            foreach (var importType in Enum.GetValues<VoyageEventType>())
            {
                var importTypeName = importType.ToName();
                EventTypes.Add(new SelectListItem() { Text = $"{importTypeName}", Value = importType.ToString() });
            }
        }
    }
}