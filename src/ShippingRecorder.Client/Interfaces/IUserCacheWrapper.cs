namespace ShippingRecorder.Client.Interfaces
{
    public interface IUserCacheWrapper : ICacheWrapper
    {
        string GetCacheKey(string prefix, string userName);
    }
}