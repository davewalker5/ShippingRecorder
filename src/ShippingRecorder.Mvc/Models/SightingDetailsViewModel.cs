using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using ShippingRecorder.Entities.Db;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ShippingRecorder.Mvc.Models
{
    public class SightingDetailsViewModel : ShippingRecorderEntityBase
    {
        public long? SightingId { get; set; }

        [DisplayName("Date")]
        [UIHint("DatePicker")]
        [DataType(DataType.Date)]
        [Required(ErrorMessage = "You must provide a date for the sighting")]
        public DateTime? Date { get; set; }

        [DisplayName("Location")]
        public long LocationId { get; set; }

        [DisplayName("New Location")]
        public string NewLocation { get; set; }

        [DisplayName("Vessel IMO")]
        [StringLength(7, MinimumLength = 7, ErrorMessage = "IMO must be 7 digits long")]
        [RegularExpression(@"^\d+$", ErrorMessage = "IMO must contain digits only")]
        [Required(ErrorMessage = "You must provide a vessel IMO")]
        public string IMO { get; set; }

        [DisplayName("Voyage")]
        public long VoyageId { get; set; }

        [DisplayName("My Voyage")]
        public bool IsMyVoyage { get; set; }

        public Sighting LastSightingAdded { get; set; }
        public string LocationErrorMessage { get; set; }
        public string Action { get; set; }
        public List<SelectListItem> Locations { get; set; }
        public List<SelectListItem> Voyages { get; set; }

        /// <summary>
        /// Set the options for the locations drop-down list
        /// </summary>
        /// <param name="locations"></param>
        public void SetLocations(List<Location> locations)
        {
            // Add the default selection, which is empty
            Locations = new List<SelectListItem>()
            {
                new SelectListItem{ Value = "0", Text = "" }
            };

            // Add the locations retrieved from the service
            if (locations != null)
            {
                Locations.AddRange(locations.Select(x =>
                                    new SelectListItem
                                    {
                                        Value = x.Id.ToString(),
                                        Text = $"{x.Name}"
                                    }));
            }
        }

        /// <summary>
        /// Set the options for the voyages drop-down list
        /// </summary>
        /// <param name="voyages"></param>
        public void SetVoyages(List<Voyage> voyages)
        {
            // Add the default selection, which is empty
            Voyages = new List<SelectListItem>()
            {
                new SelectListItem{ Value = "0", Text = "" }
            };

            // Add the voyages retrieved from the service
            if (voyages != null)
            {
                Voyages.AddRange(voyages.Select(x =>
                                    new SelectListItem
                                    {
                                        Value = x.Id.ToString(),
                                        Text = $"{x}"
                                    }));
            }
        }
    }
}
