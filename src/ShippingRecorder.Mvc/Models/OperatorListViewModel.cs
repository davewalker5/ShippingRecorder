using ShippingRecorder.Entities.Db;

namespace ShippingRecorder.Mvc.Models
{
    public class OperatorListViewModel : PaginatedViewModelBase
    {
        public IEnumerable<Operator> Operators { get; private set; }

        /// <summary>
        /// Set the list of locations to be exposed by this view model
        /// </summary>
        /// <param name="locations"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        public void SetOperators(IEnumerable<Operator> locations, int pageNumber, int pageSize)
        {
            Operators = locations ?? [];
            PageNumber = pageNumber;
            SetPreviousNextEnabled(Operators.Count(), pageNumber, pageSize);
        }
    }
}