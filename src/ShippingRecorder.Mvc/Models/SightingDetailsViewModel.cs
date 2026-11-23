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

        [DisplayName("IMO or MMSI")]
        [RegularExpression(@"^(?:[0-9]{7}|[0-9]{9})$", ErrorMessage = "Enter a 7-digit IMO or a 9-digit MMSI")]
        [Required(ErrorMessage = "You must provide a vessel identifier")]
        public string Identifier { get; set; }

        [DisplayName("Voyage")]
        public long VoyageId { get; set; }
        public string VoyageName { get; set; }

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
