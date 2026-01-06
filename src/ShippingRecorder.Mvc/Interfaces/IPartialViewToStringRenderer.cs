namespace ShippingRecorder.Mvc.Interfaces
{
    public interface IPartialViewToStringRenderer
    {
        Task<string> RenderPartialViewToStringAsync(string viewName, object model);
    }
}