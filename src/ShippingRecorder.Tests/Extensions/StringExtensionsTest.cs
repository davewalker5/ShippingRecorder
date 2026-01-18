using Microsoft.VisualStudio.TestTools.UnitTesting;
using ShippingRecorder.BusinessLogic.Extensions;
using ShippingRecorder.Entities.Exceptions;

namespace ShippingRecorder.Tests.Extensions
{
    [TestClass]
    public class StringExtensionsTest
    {
        [TestMethod]
        public void CleanTest()
            => Assert.AreEqual("AbC 1234 d &*^", "   AbC    1234 d &*^ ".Clean());

        [TestMethod]
        public void CleanCodeTest()
            => Assert.AreEqual("ABC123", " aBc  123 ".CleanCode());

        [TestMethod]
        public void TitleCaseTest()
            => Assert.AreEqual("Abc 123 Zhtyiu*", " aBc  123 zhtyIU* ".TitleCase());

        [TestMethod]
        public void ValidateNumericTest()
            => Assert.IsTrue("1234567890".ValidateNumeric(1, int.MaxValue));

        [TestMethod]
        public void ValidateShortNumericFailsTest()
            => Assert.IsFalse("123".ValidateNumeric(4, int.MaxValue));

        [TestMethod]
        public void ValidateLongNumericFailsTest()
            => Assert.IsFalse("123".ValidateNumeric(1, 2));

        [TestMethod]
        public void ValidateNonNumericTest()
            => Assert.IsFalse("1.4566".ValidateNumeric(1, int.MaxValue));

        [TestMethod]
        public void ValidateFailureThrowsExceptionNumericTest()
            => Assert.Throws<InvalidVesselIdentifierException>(() => "ABC1234".ValidateNumericAndThrow<InvalidVesselIdentifierException>(7, 7));
    }
}