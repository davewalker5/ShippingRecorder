using Microsoft.VisualStudio.TestTools.UnitTesting;
using ShippingRecorder.BusinessLogic.Extensions;
using ShippingRecorder.Entities.Exceptions;

namespace ShippingRecorder.Tests.Extensions
{
    [TestClass]
    public class IntegerExtensionsTest
    {
        [TestMethod]
        public void ValidateNullPassesTest()
        {
            int? i = null;
            Assert.IsTrue(i.ValidateInteger(1, int.MaxValue, true));
        }

        [TestMethod]
        public void ValidateNullFailsTest()
        {
            int? i = null;
            Assert.IsFalse(i.ValidateInteger(1, int.MaxValue, false));
        }

        [TestMethod]
        public void ValidateInRangeTest()
        {
            int? i = 123;
            Assert.IsTrue(i.ValidateInteger(1, int.MaxValue, false));
        }

        [TestMethod]
        public void ValidateTooSmallTest()
        {
            int? i = 393;
            Assert.IsFalse(i.ValidateInteger(1000, int.MaxValue, false));
        }

        [TestMethod]
        public void ValidateTooLargeTest()
        {
            int? i = 34845;
            Assert.IsFalse(i.ValidateInteger(1, 100, false));
        }

        [TestMethod]
        public void ValidateFailureThrowsExceptionNumericTest()
        {
            int? i = null;
            Assert.Throws<InvalidIMOException>(() => i.ValidateIntegerAndThrow<InvalidIMOException>(1, int.MaxValue, false));
        }
    }
}