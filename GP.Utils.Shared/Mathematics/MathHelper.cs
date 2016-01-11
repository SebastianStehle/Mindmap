// ==========================================================================
// MathHelper.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System.Numerics;
using System.Runtime.CompilerServices;

// ReSharper disable CompareOfFloatsByEqualityOperator

namespace GP.Utils.Mathematics
{
    /// <summary>
    /// Defines helper methods for math operations.
    /// </summary>
    public static class MathHelper
    {
        /// <summary>
        /// A positive infinity vector.
        /// </summary>
        public static readonly Vector2 PositiveInfinityVector = new Vector2(float.PositiveInfinity, float.PositiveInfinity);

        /// <summary>
        /// A negative infinity vector.
        /// </summary>
        public static readonly Vector2 NegativeInfinityVector = new Vector2(float.NegativeInfinity, float.NegativeInfinity);

        /// <summary>
        /// Interpolates between the first and the second value depending on the fraction value.
        /// </summary>
        /// <param name="fraction">The fraction.</param>
        /// <param name="l">The first value.</param>
        /// <param name="r">The second value.</param>
        /// <returns>s
        /// The resulting value.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 Interpolate(float fraction, Vector2 l, Vector2 r)
        {
            return new Vector2(Interpolate(fraction, l.X, r.X), Interpolate(fraction, l.Y, r.Y));
        }

        /// <summary>
        /// Interpolates between the first and the right value depending on the fraction value.
        /// </summary>
        /// <param name="fraction">The fraction.</param>
        /// <param name="l">The first value.</param>
        /// <param name="r">The second value.</param>
        /// <returns>
        /// The resulting value.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Interpolate(float fraction, float l, float r)
        {
            return ((r - l) * fraction) + l;
        }

        /// <summary>
        /// Interpolates between the first and the right value depending on the fraction value.
        /// </summary>
        /// <param name="fraction">The fraction.</param>
        /// <param name="l">The first value.</param>
        /// <param name="r">The second value.</param>
        /// <returns>
        /// The resulting value.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double Interpolate(double fraction, double l, double r)
        {
            return ((r - l) * fraction) + l;
        }

        /// <summary>
        /// Rounds the vector
        /// </summary>
        /// <param name="value">The vector to round.</param>
        /// <returns>
        /// The rounded value.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 Round(this Vector2 value)
        {
            return new Vector2((float)System.Math.Round(value.X), (float)System.Math.Round(value.Y));
        }

        /// <summary>
        /// Rounds the vector
        /// </summary>
        /// <param name="value">The vector to round.</param>
        /// <returns>
        /// The rounded value.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Round(ref Vector2 value)
        {
            value.X = (float)System.Math.Round(value.X);
            value.Y = (float)System.Math.Round(value.Y);
        }

        /// <summary>
        /// Rounds the vector to a multiple of two.
        /// </summary>
        /// <param name="value">The vector to round.</param>
        /// <returns>
        /// The rounded value.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void RoundToMultipleOfTwo(ref Vector2 value)
        {
            value.X = (float)System.Math.Round(value.X);
            value.Y = (float)System.Math.Round(value.Y);

            if (value.Y % 2 == 1)
            {
                value.Y += 1;
            }

            if (value.X % 2 == 1)
            {
                value.X += 1;
            }
        }

        /// <summary>
        /// Rounds the vector to a multiple of two.
        /// </summary>
        /// <param name="value">The vector to round.</param>
        /// <returns>
        /// The rounded value.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 RoundToMultipleOfTwo(Vector2 value)
        {
            value.X = (float)System.Math.Round(value.X);
            value.Y = (float)System.Math.Round(value.Y);

            if (value.Y % 2 == 1)
            {
                value.Y += 1;
            }

            if (value.X % 2 == 1)
            {
                value.X += 1;
            }

            return value;
        }

        /// <summary>
        /// Rounds the vector
        /// </summary>
        /// <param name="value">The vector to round.</param>
        /// <returns>
        /// The rounded value.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 Round(this Vector3 value)
        {
            return new Vector3((float)System.Math.Round(value.X), (float)System.Math.Round(value.Y), (float)System.Math.Round(value.Z));
        }

        /// <summary>
        /// Rounds the vector
        /// </summary>
        /// <param name="value">The vector to round.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Round(ref Vector3 value)
        {
            value.X = (float)System.Math.Round(value.X);
            value.Y = (float)System.Math.Round(value.Y);
            value.Z = (float)System.Math.Round(value.Z);
        }

        /// <summary>
        /// Determines if two points are about equal.
        /// </summary>
        /// <param name="l">The first point.</param>
        /// <param name="r">The second point.</param>
        /// <returns>
        /// True, if two point values are more or less equal.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool AboutEqual(Vector2 l, Vector2 r)
        {
            return AboutEqual(l.X, r.X) && AboutEqual(l.Y, r.Y);
        }

        /// <summary>
        /// Determines if two double values are about equal.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <returns>
        /// True, if two double values are more or less equal.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool AboutEqual(double x, double y)
        {
            double epsilon = System.Math.Max(System.Math.Abs(x), System.Math.Abs(y)) * 1E-15;

            return System.Math.Abs(x - y) <= epsilon;
        }

        /// <summary>
        /// Transforms a vector by the given matrix.
        /// </summary>
        /// <param name="position">The source vector.</param>
        /// <param name="matrix">The transformation matrix.</param>
        /// <returns>The transformed vector.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 Transform(Vector2 position, Matrix3x2 matrix)
        {
            return new Vector2(
                (position.X * matrix.M11) + (position.Y * matrix.M21) + matrix.M31,
                (position.X * matrix.M12) + (position.Y * matrix.M22) + matrix.M32);
        }
    }
}
