// ==========================================================================
// ColorsHelperTest.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using GP.Utils.Mathematics;
using Xunit;

namespace Tests.Facts
{
    public class ColorsHelperTest
    {
        [Fact]
        public void ColorToRGBString1()
        {
            string actual = ColorsVectorHelper.ConvertToRGBString(0xFF0000);

            Assert.Equal("#FF0000", actual);
        }

        [Fact]
        public void ColorToRGBString2()
        {
            string actual = ColorsVectorHelper.ConvertToRGBString(0x0000FF);

            Assert.Equal("#0000FF", actual);
        }
    }
}
