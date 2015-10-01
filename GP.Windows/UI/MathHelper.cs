// ==========================================================================
// MathHelper.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using Windows.Foundation;
using Windows.UI.Xaml.Media;

namespace GP.Windows.UI
{
    /// <summary>
    /// Defines helper methods for math operations.
    /// </summary>
    public static class MathHelper
    {
        /// <summary>
        /// Calculates the length between two points.
        /// </summary>
        /// <param name="l">The first point.</param>
        /// <param name="r">The second point.</param>
        /// <returns>
        /// The length.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double Length(Point l, Point r)
        {
            double dx = l.X - r.X;
            double dy = l.Y - r.Y;

            return Math.Sqrt((dx * dx) + (dy * dy));
        }

        /// <summary>
        /// Calculates the squared length between two points.
        /// </summary>
        /// <param name="l">The first point.</param>
        /// <param name="r">The second point.</param>
        /// <returns>
        /// The squared length.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double LengthSquared(Point l, Point r)
        {
            double dx = l.X - r.X;
            double dy = l.Y - r.Y;

            return (dx * dx) + (dy * dy);
        }

        /// <summary>
        /// Gets or sets the position of the rectangle.
        /// </summary>
        /// <param name="rect">The rectangle.</param>
        /// <returns>
        /// The position of the rectangle.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Point Position(this Rect rect)
        {
            return new Point(rect.X, rect.Y);
        }

        /// <summary>
        /// Gets or sets the position of the transform.
        /// </summary>
        /// <param name="transform">The transform.</param>
        /// <returns>
        /// The position of the transform.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Point Position(this TranslateTransform transform)
        {
            return new Point(transform.X, transform.Y);
        }

        /// <summary>
        /// Gets or sets the size of the rectangle.
        /// </summary>
        /// <param name="rect">The rectangle.</param>
        /// <returns>
        /// The size of of the rectangle.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Size Size(this Rect rect)
        {
            return new Size(rect.Width, rect.Height);
        }

        /// <summary>
        /// Calculates the center of the target rect.
        /// </summary>
        /// <param name="rect">The rect where to get the center from.</param>
        /// <returns>
        /// The center.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Point Center(this Rect rect)
        {
            return new Point(rect.CenterX(), rect.CenterY());
        }

        /// <summary>
        /// Calculates the center of the of the x-Coordinate of the target rect.
        /// </summary>
        /// <param name="rect">The rect where to get the center from.</param>
        /// <returns>
        /// The center.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double CenterX(this Rect rect)
        {
            return rect.X + (0.5 * rect.Width);
        }

        /// <summary>
        /// Calculates the center of the of the y-Coordinate of the target rect.
        /// </summary>
        /// <param name="rect">The rect where to get the center from.</param>
        /// <returns>
        /// The center.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double CenterY(this Rect rect)
        {
            return rect.Y + (0.5 * rect.Height);
        }

        /// <summary>
        /// Interpolates between the first and the second rectangle depending on the fraction value.
        /// </summary>
        /// <param name="fraction">The fraction.</param>
        /// <param name="l">The first rectangle.</param>
        /// <param name="r">The second rectangle.</param>
        /// <returns>
        /// The resulting rectangle.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Rect Interpolate(double fraction, Rect l, Rect r)
        {
            return new Rect(Interpolate(fraction, l.X, r.X), Interpolate(fraction, l.Y, r.Y), Interpolate(fraction, l.Width, r.Width), Interpolate(fraction, l.Height, r.Height));
        }

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
        /// Interpolates between the first and the second value depending on the fraction value.
        /// </summary>
        /// <param name="fraction">The fraction.</param>
        /// <param name="l">The first value.</param>
        /// <param name="r">The second value.</param>
        /// <returns>
        /// The resulting value.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Point Interpolate(double fraction, Point l, Point r)
        {
            return new Point(Interpolate(fraction, l.X, r.X), Interpolate(fraction, l.Y, r.Y));
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
            return new Vector2((float)Math.Round(value.X), (float)Math.Round(value.Y));
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
            value.X = (float)Math.Round(value.X);
            value.Y = (float)Math.Round(value.Y);
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
            return new Vector3((float)Math.Round(value.X), (float)Math.Round(value.Y), (float)Math.Round(value.Z));
        }

        /// <summary>
        /// Rounds the vector
        /// </summary>
        /// <param name="value">The vector to round.</param>
        /// <returns>
        /// The rounded value.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Round(ref Vector3 value)
        {
            value.X = (float)Math.Round(value.X);
            value.Y = (float)Math.Round(value.Y);
            value.Z = (float)Math.Round(value.Z);
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
        /// Determines if two points are about equal.
        /// </summary>
        /// <param name="l">The first point.</param>
        /// <param name="r">The second point.</param>
        /// <returns>
        /// True, if two point values are more or less equal.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool AboutEqual(Point l, Point r)
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
            double epsilon = Math.Max(Math.Abs(x), Math.Abs(y)) * 1E-15;

            return Math.Abs(x - y) <= epsilon;
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
