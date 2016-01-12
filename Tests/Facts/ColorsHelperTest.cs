// ==========================================================================
// ColorsHelperTest.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System.Numerics;
using GP.Utils.Mathematics;
using Xunit;

namespace Tests.Facts
{
    public class ColorsHelperTest
    {
        [Fact]
        public void Color_To_String1()
        {
            string actual = ColorsVectorHelper.ConvertToRGBString(0xFF0000);

            Assert.Equal("#FF0000", actual);
        }

        [Fact]
        public void Vector_To_Int1()
        {
            int actual = ColorsVectorHelper.ConvertToInt(new Vector3(1, 0, 0));

            Assert.Equal(0xFF0000, actual);
        }

        [Fact]
        public void Color_To_String2()
        {
            string actual = ColorsVectorHelper.ConvertToRGBString(0x0000FF);

            Assert.Equal("#0000FF", actual);
        }

        [Fact]
        public void Vector_To_Int2()
        {
            int actual = ColorsVectorHelper.ConvertToInt(new Vector3(0, 0, 1));

            Assert.Equal(0x0000FF, actual);
        }
    }
}
