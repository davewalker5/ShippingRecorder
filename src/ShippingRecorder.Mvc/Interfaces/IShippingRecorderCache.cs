using ShippingRecorder.Client.Interfaces;

namespace ShippingRecorder.Mvc.Interfaces
{
    public interface IShippingRecorderCache : IUserCacheWrapper
    {
        DateTime GetEventDate(string userName);
        void SetEventDate(string userName, DateTime date);
        void ClearEventDate(string userName);
    }
}