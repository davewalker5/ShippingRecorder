using Microsoft.Extensions.Caching.Memory;
using ShippingRecorder.Client.ApiClient;
using ShippingRecorder.Client.Interfaces;
using ShippingRecorder.Mvc.Interfaces;

namespace HealthTracker.Mvc.Helpers
{
    public class ShippingRecorderCache : UserCacheWrapper, IShippingRecorderCache, IDisposable
    {
        private const int DefaultDuration = 86400;
        private const string EventDateKey = "EventDate";
        
        private bool _disposed = false;

        public ShippingRecorderCache(MemoryCacheOptions options) : base(options)
        {
        }

        /// <summary>
        /// Get the cached event date
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public DateTime GetEventDate(string userName)
            => Get<DateTime?>(GetCacheKey(EventDateKey, userName)) ?? DateTime.Today;

        /// <summary>
        /// Set the cached event date
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public void SetEventDate(string userName, DateTime date)
            => Set<DateTime>(GetCacheKey(EventDateKey, userName), date, DefaultDuration);

        /// <summary>
        /// Clear the cached event key from the cache
        /// </summary>
        public void ClearEventDate(string userName)
            => Remove(GetCacheKey(EventDateKey, userName));

        /// <summary>
        /// IDisposable implementation
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                _disposed = true;
                base.Dispose(disposing);
            }
        }
    }
}