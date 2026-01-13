using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using ShippingRecorder.Entities.Db;

namespace ShippingRecorder.Mvc.Models
{
    public class VoyageSearchViewModel : PaginatedViewModelBase
    {
        [DisplayName("Operator")]
        [Required(ErrorMessage = "You must select an operator")]
        public long? OperatorId { get; set; }

        public IList<SelectListItem> Operators { get; set; }
        public List<Voyage> Voyages { get; private set; }
        public string Message { get; set; } = "";

        /// <summary>
        /// Set the collection of voyages that are exposed to the view
        /// </summary>
        /// <param name="sightings"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        public void SetVoyages(List<Voyage> voyages, int pageNumber, int pageSize)
        {
            Voyages = voyages ?? [];
            HasNoMatchingResults = !Voyages.Any();
            PageNumber = pageNumber;
            SetPreviousNextEnabled(Voyages.Count, pageNumber, pageSize);
        }
    }
}
