// ==========================================================================
// MathHelperTest.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System;
using System.Numerics;
using GP.Utils.Mathematics;
using Xunit;
// ReSharper disable ReturnValueOfPureMethodIsNotUsed
// ReSharper disable ConvertToConstant.Local

namespace Tests.Facts
{
    public class MathHelperTest
    {
        private readonly Random random = new Random();

        public float Random10()
        {
            return (float)(random.NextDouble() * 10);
        }

        [Fact]
        public void Constants()
        {
            float.IsPositiveInfinity(MathHelper.PositiveInfinityVector2.X);
            float.IsPositiveInfinity(MathHelper.PositiveInfinityVector2.Y);

            float.IsNegativeInfinity(MathHelper.NegativeInfinityVector2.X);
            float.IsNegativeInfinity(MathHelper.NegativeInfinityVector2.Y);
        }

        [Fact]
        public void RoundToMultipleOfTwo()
        {
            Vector2 v1 = new Vector2(1.2f, 5.2f);
            Vector2 v2 = new Vector2(1.6f, 5.7f);

            Assert.Equal(new Vector2(2, 6), MathHelper.RoundToMultipleOfTwo(v1));
            Assert.Equal(new Vector2(2, 6), MathHelper.RoundToMultipleOfTwo(v2));

            MathHelper.RoundToMultipleOfTwo(ref v1);
            MathHelper.RoundToMultipleOfTwo(ref v2);

            Assert.Equal(new Vector2(2, 6), v1);
            Assert.Equal(new Vector2(2, 6), v2);
        }

        [Fact]
        public void RoundVector2_ByValue_ByRef()
        {
            Vector2 source = new Vector2(Random10(), Random10());
            Vector2 result = source.Round();

            Assert.Equal(Math.Round(source.X), result.X);
            Assert.Equal(Math.Round(source.Y), result.Y);

            result = source;

            MathHelper.Round(ref result);

            Assert.Equal(Math.Round(source.X), result.X);
            Assert.Equal(Math.Round(source.Y), result.Y);
        }

        [Fact]
        public void RoundVector3_ByValue_ByRef()
        {
            Vector3 source = new Vector3(Random10(), Random10(), Random10());
            Vector3 result = source.Round();

            Assert.Equal(Math.Round(source.X), result.X);
            Assert.Equal(Math.Round(source.Y), result.Y);
            Assert.Equal(Math.Round(source.Z), result.Z);

            result = source;

            MathHelper.Round(ref result);

            Assert.Equal(Math.Round(source.X), result.X);
            Assert.Equal(Math.Round(source.Y), result.Y);
            Assert.Equal(Math.Round(source.Z), result.Z);
        }

        [Fact]
        public void Interpolate_Float()
        {
            float l = 10;
            float r = 60;

            Assert.Equal(l, MathHelper.Interpolate(-1, l, r));
            Assert.Equal(r, MathHelper.Interpolate( 2, l, r));
            Assert.Equal(20, MathHelper.Interpolate(0.2f, l, r));
        }

        [Fact]
        public void Interpolate_Double()
        {
            double l = 10;
            double r = 60;

            Assert.Equal(l, MathHelper.Interpolate(-1, l, r));
            Assert.Equal(r, MathHelper.Interpolate( 2, l, r));

            Assert.Equal(20, MathHelper.Interpolate(0.2, l, r));
        }

        [Fact]
        public void Interpolate_Vector2()
        {
            Vector2 l = new Vector2(10, 50);
            Vector2 r = new Vector2(60, 150);

            Assert.Equal(l, MathHelper.Interpolate(-1, l, r));
            Assert.Equal(r, MathHelper.Interpolate( 2, l, r));

            Assert.Equal(new Vector2(20, 70), MathHelper.Interpolate(0.2f, l, r));
        }

        [Fact]
        public void AboutEquals_Float_ByEpsilon_ReturnsTrue()
        {
            float v = 10;

            Assert.True(MathHelper.AboutEqual(v, v + float.Epsilon));
        }

        [Fact]
        public void AboutEquals_Double_ByEpsilon_ReturnsTrue()
        {
            double v = 10;

            Assert.True(MathHelper.AboutEqual(v, v + double.Epsilon));
        }

        [Fact]
        public void AboutEquals_Vector2_ByEpsilon_ReturnsTrue()
        {
            Vector2 v = new Vector2(10, 50);

            Assert.True(MathHelper.AboutEqual(v, v + new Vector2(float.Epsilon, float.Epsilon)));
        }
    }
}
