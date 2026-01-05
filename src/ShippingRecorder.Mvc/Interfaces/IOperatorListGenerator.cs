using Microsoft.AspNetCore.Mvc.Rendering;

namespace ShippingRecorder.Mvc.Interfaces
{
    public interface IOperatorListGenerator
    {
        Task<IList<SelectListItem>> Create();
    }
}