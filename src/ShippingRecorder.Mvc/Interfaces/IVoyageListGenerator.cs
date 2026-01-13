using Microsoft.AspNetCore.Mvc.Rendering;

namespace ShippingRecorder.Mvc.Interfaces
{
    public interface IVoyageListGenerator
    {
        Task<IList<SelectListItem>> Create();
    }
}