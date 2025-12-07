using Microsoft.VisualStudio.TestTools.UnitTesting;
using ShippingRecorder.BusinessLogic.Extensions;
using ShippingRecorder.Entities.Exceptions;

namespace ShippingRecorder.Tests.Extensions
{
    [TestClass]
    public class DecimalExtensionsTest
    {
        [TestMethod]
        public void ValidateNullPassesTest()
        {
            decimal? i = null;
            Assert.IsTrue(i.ValidateDecimal(1, decimal.MaxValue, true));
        }

        [TestMethod]
        public void ValidateNullFailsTest()
        {
            decimal? i = null;
            Assert.IsFalse(i.ValidateDecimal(1, decimal.MaxValue, false));
        }

        [TestMethod]
        public void ValidateInRangeTest()
        {
            decimal? i = 123.234987M;
            Assert.IsTrue(i.ValidateDecimal(1, decimal.MaxValue, false));
        }

        [TestMethod]
        public void ValidateTooSmallTest()
        {
            decimal? i = 393.1238476M;
            Assert.IsFalse(i.ValidateDecimal(1000, decimal.MaxValue, false));
        }

        [TestMethod]
        public void ValidateTooLargeTest()
        {
            decimal? i = 34845.12915M;
            Assert.IsFalse(i.ValidateDecimal(1, 100, false));
        }

        [TestMethod]
        public void ValidateFailureThrowsExceptionNumericTest()
        {
            decimal? i = null;
            Assert.Throws<InvalidIMOException>(() => i.ValidateDecimalAndThrow<InvalidIMOException>(1, decimal.MaxValue, false));
        }
    }
}