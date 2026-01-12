using Microsoft.AspNetCore.Mvc.Rendering;

namespace ShippingRecorder.Mvc.Interfaces
{
    public interface IVesselListGenerator
    {
        Task<IList<SelectListItem>> Create();
    }
}