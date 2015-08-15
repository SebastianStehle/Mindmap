// ==========================================================================
// ColorsHelperTest.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================
using GP.Windows.UI;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;

namespace UnitTests.Tests
{
    [TestClass]
    public class ColorsHelperTest
    {
        [TestMethod]
        public void ColorToRGBString1()
        {
            string actual = ColorsHelper.ConvertToRGBString(0xFF0000);

            Assert.AreEqual("#FF0000", actual);
        }

        [TestMethod]
        public void ColorToRGBString2()
        {
            string actual = ColorsHelper.ConvertToRGBString(0x0000FF);

            Assert.AreEqual("#0000FF", actual);
        }
    }
}
