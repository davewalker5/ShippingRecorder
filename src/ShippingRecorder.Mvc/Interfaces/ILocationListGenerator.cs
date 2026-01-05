using Microsoft.AspNetCore.Mvc.Rendering;

namespace ShippingRecorder.Mvc.Interfaces
{
    public interface ILocationListGenerator
    {
        Task<IList<SelectListItem>> Create();
    }
}