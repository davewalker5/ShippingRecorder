using System.ComponentModel;
using Microsoft.AspNetCore.Mvc.Rendering;
using ShippingRecorder.Entities.Db;

namespace ShippingRecorder.Mvc.Models
{
    public class PortListViewModel : PaginatedViewModelBase
    {
        public IList<SelectListItem> Countries { get; set; }
        public IEnumerable<Port> Ports { get; private set; }

        [DisplayName("Country")]
        public long CountryId { get; set; }

        public string Message { get; set; }

        /// <summary>
        /// Set the list of ports to be exposed by this view model
        /// </summary>
        /// <param name="ports"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        public void SetPorts(IEnumerable<Port> ports, int pageNumber, int pageSize)
        {
            Ports = ports ?? [];
            PageNumber = pageNumber;
            SetPreviousNextEnabled(Ports.Count(), pageNumber, pageSize);
        }
    }
}