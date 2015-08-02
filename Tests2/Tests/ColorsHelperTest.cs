// ==========================================================================
// DocumentStoreTest.cs
// RavenMind Application
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using RavenMind.Model;

namespace RavenMind.Tests
{
    [TestClass]
    public class ColorsHelperTest
    {
        [TestMethod]
        public void TestColorToRGBString1()
        {
            string actual = ColorsHelper.ConvertToRGBString(0xFF0000);

            Assert.AreEqual("#FF0000", actual);
        }

        [TestMethod]
        public void TestColorToRGBString2()
        {
            string actual = ColorsHelper.ConvertToRGBString(0x0000FF);

            Assert.AreEqual("#0000FF", actual);
        }
    }
}
