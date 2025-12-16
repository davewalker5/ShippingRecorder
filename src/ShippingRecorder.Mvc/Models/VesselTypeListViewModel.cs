using ShippingRecorder.Entities.Db;

namespace ShippingRecorder.Mvc.Models
{
    public class VesselTypeListViewModel : PaginatedViewModelBase
    {
        public IEnumerable<VesselType> VesselTypes { get; private set; }

        /// <summary>
        /// Set the list of locations to be exposed by this view model
        /// </summary>
        /// <param name="locations"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        public void SetVesselTypes(IEnumerable<VesselType> locations, int pageNumber, int pageSize)
        {
            VesselTypes = locations ?? [];
            PageNumber = pageNumber;
            SetPreviousNextEnabled(VesselTypes.Count(), pageNumber, pageSize);
        }
    }
}