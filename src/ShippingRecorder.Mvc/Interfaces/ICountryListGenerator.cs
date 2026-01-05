using Microsoft.AspNetCore.Mvc.Rendering;

namespace ShippingRecorder.Mvc.Interfaces
{
    public interface ICountryListGenerator
    {
        Task<IList<SelectListItem>> Create();
    }
}