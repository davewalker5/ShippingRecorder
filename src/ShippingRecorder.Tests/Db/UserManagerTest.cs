using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ShippingRecorder.BusinessLogic.Factory;
using ShippingRecorder.Data;
using ShippingRecorder.Entities.Db;
using ShippingRecorder.Entities.Exceptions;
using ShippingRecorder.Tests.Mocks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ShippingRecorder.Tests
{
    [TestClass]
    public class UserManagerTest
    {
        private const string UserName = "Some User";
        private const string Password = "password";
        private const string UpdatedPassword = "newpassword";

        private ShippingRecorderFactory _factory;
        private User _user;

        [TestInitialize]
        public void TestInitialize()
        {
            ShippingRecorderDbContext context = ShippingRecorderDbContextFactory.CreateInMemoryDbContext();
            _factory = new ShippingRecorderFactory(context, new MockFileLogger());

            _user = Task.Run(() => _factory.Users.AddUserAsync(UserName, Password)).Result;
        }

        [TestMethod]
        public async Task AddExistingUserAsyncTest()
            => await Assert.ThrowsAsync<UserExistsException>(() => _factory.Users.AddUserAsync(UserName, Password));

        [TestMethod]
        public async Task DeleteUserAsyncTest()
        {
            await _factory.Users.DeleteUserAsync(UserName);
            List<User> users = await _factory.Users.GetUsersAsync().ToListAsync();
            Assert.IsFalse(users.Any());
        }

        [TestMethod]
        public async Task GetUserByIdAsyncTest()
        {
            User user = await _factory.Users.GetUserAsync(_user.Id);
            Assert.AreEqual(UserName, user.UserName);
            Assert.AreNotEqual(Password, user.Password);
        }

        [TestMethod]
        public async Task GetMissingUserByIdAsyncTest()
            => await Assert.ThrowsAsync<UserNotFoundException>(() => _factory.Users.GetUserAsync(-1));

        [TestMethod]
        public async Task GetAllUsersAsyncTest()
        {
            List<User> users = await _factory.Users.GetUsersAsync().ToListAsync();
            Assert.HasCount(1, users);
            Assert.AreEqual(UserName, users.First().UserName);
            Assert.AreNotEqual(Password, users.First().Password);
        }

        [TestMethod]
        public async Task AuthenticateAsyncTest()
        {
            bool authenticated = await _factory.Users.AuthenticateAsync(UserName, Password);
            Assert.IsTrue(authenticated);
        }

        [TestMethod]
        public async Task FailedAuthenticationTest()
        {
            bool authenticated = await _factory.Users.AuthenticateAsync(UserName, "the wrong password");
            Assert.IsFalse(authenticated);
        }

        [TestMethod]
        public async Task SetPassswordAsyncTest()
        {
            await _factory.Users.SetPasswordAsync(UserName, UpdatedPassword);
            bool authenticated = await _factory.Users.AuthenticateAsync(UserName, UpdatedPassword);
            Assert.IsTrue(authenticated);
        }
    }
}
