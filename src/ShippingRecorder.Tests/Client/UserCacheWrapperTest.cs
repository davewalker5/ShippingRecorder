using System.Linq;
using System.Threading;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ShippingRecorder.Client.ApiClient;
using ShippingRecorder.Client.Interfaces;
using ShippingRecorder.Entities.Db;
using ShippingRecorder.Tests.Mocks;

namespace ShippingRecorder.Tests.Client
{
    [TestClass]
    public class UserCacheWrapperTest
    {
        private const int MinimumDuration = 30;
        private const int MaximumDuration = 60;

        private IUserCacheWrapper _cache;
        private string _baseKey;
        private string _userName;
        private Location _data;

        [TestInitialize]
        public void Initialise()
        {
            _cache = new UserCacheWrapper(new MemoryCacheOptions());
            _cache.Clear();
            _baseKey = DataGenerator.RandomWord(20, 50);
            _userName = DataGenerator.RandomWord(20, 50);
            _data = DataGenerator.CreateLocation();
        }

        [TestCleanup]
        public void CleanUp()
            => _cache.Dispose();

        [TestMethod]
        public void GetCacheKeyTest()
        {
            var expected = $"{_baseKey}.{_userName}";
            var actual = _cache.GetCacheKey(_baseKey, _userName);
            Assert.AreEqual(expected, actual);
        }


        [TestMethod]
        public void SetAndGetTest()
        {
            var duration = DataGenerator.RandomInt(MinimumDuration, MaximumDuration);

            var key = _cache.GetCacheKey(_baseKey, _userName);
            _cache.Set<Location>(key, _data, duration);

            var retrieved = _cache.Get<Location>(key);

            Assert.AreEqual(_data.Id, retrieved.Id);
            Assert.AreEqual(_data.Name, retrieved.Name);
        }

        [TestMethod]
        public void RemoveTest()
        {
            var duration = DataGenerator.RandomInt(MinimumDuration, MaximumDuration);
            var key = _cache.GetCacheKey(_baseKey, _userName);
            _cache.Set<Location>(key, _data, duration);
            _cache.Remove(key);

            var retrieved = _cache.Get<Location>(key);

            Assert.IsNull(retrieved);
        }

        [TestMethod]
        public void ClearTest()
        {
            var duration = DataGenerator.RandomInt(MinimumDuration, MaximumDuration);
            var key = _cache.GetCacheKey(_baseKey, _userName);
            _cache.Set<Location>(key, _data, duration);
            _cache.Clear();

            var retrieved = _cache.Get<Location>(key);

            Assert.IsNull(retrieved);
        }

        [TestMethod]
        public void ExpiryTest()
        {
            var key = _cache.GetCacheKey(_baseKey, _userName);
            _cache.Set<Location>(key, _data, 1);
            Thread.Sleep(1010);

            var retrieved = _cache.Get<Location>(key);

            Assert.IsNull(retrieved);
        }

        [TestMethod]
        public void GetKeysTest()
        {
            var duration = DataGenerator.RandomInt(MinimumDuration, MaximumDuration);
            var key = _cache.GetCacheKey(_baseKey, _userName);
            _cache.Set<Location>(key, _data, duration);

            var keys = _cache.GetKeys();

            Assert.IsNotNull(keys);
            Assert.HasCount(1, keys);
            Assert.AreEqual(key, keys.First());
        }

        [TestMethod]
        public void GetFilteredKeysTest()
        {
            var duration = DataGenerator.RandomInt(MinimumDuration, MaximumDuration);
            var key = _cache.GetCacheKey(_baseKey, _userName);
            _cache.Set<Location>(key, _data, duration);

            var keys = _cache.GetFilteredKeys(key[..20]);

            Assert.IsNotNull(keys);
            Assert.HasCount(1, keys);
            Assert.AreEqual(key, keys.First());
        }

        [TestMethod]
        public void GetFilteredKeysForMissingKeyTest()
        {
            var duration = DataGenerator.RandomInt(MinimumDuration, MaximumDuration);
            var key = _cache.GetCacheKey(_baseKey, _userName);
            _cache.Set<Location>(key, _data, duration);

            var keys = _cache.GetFilteredKeys("This Key Will Be Missing From The Original Random Word");

            Assert.IsNotNull(keys);
            Assert.IsEmpty(keys);
        }
    }
}