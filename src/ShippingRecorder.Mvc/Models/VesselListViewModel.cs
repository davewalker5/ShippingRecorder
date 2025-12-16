using ShippingRecorder.Entities.Db;

namespace ShippingRecorder.Mvc.Models
{
    public class VesselListViewModel : PaginatedViewModelBase
    {
        public IEnumerable<Vessel> Vessels { get; private set; }

        /// <summary>
        /// Set the list of vessels to be exposed by this view model
        /// </summary>
        /// <param name="locations"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        public void SetVessels(IEnumerable<Vessel> locations, int pageNumber, int pageSize)
        {
            Vessels = locations ?? [];
            PageNumber = pageNumber;
            SetPreviousNextEnabled(Vessels.Count(), pageNumber, pageSize);
        }
    }
}