using Microsoft.AspNetCore.Mvc.Rendering;

namespace ShippingRecorder.Mvc.Interfaces
{
    public interface IVesselTypeListGenerator
    {
        Task<IList<SelectListItem>> Create();
    }
}