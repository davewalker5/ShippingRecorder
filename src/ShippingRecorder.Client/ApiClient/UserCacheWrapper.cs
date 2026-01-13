using System;
using Microsoft.Extensions.Caching.Memory;
using ShippingRecorder.Client.Interfaces;

namespace ShippingRecorder.Client.ApiClient
{
    public class UserCacheWrapper : CacheWrapper, IUserCacheWrapper, IDisposable
    {
        private bool _disposed = false;

        public UserCacheWrapper(MemoryCacheOptions options) : base(options)
        {
        }

        /// <summary>
        /// Given a base key and username, return a key for caching data tagged with the username
        /// </summary>
        /// <param name="baseKey"></param>
        /// <param name="userName"></param>
        /// <returns></returns>
        public string GetCacheKey(string prefix, string userName)
            => $"{prefix}.{userName}";

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