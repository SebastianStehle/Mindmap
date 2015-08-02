// ==========================================================================
// MathHelper.cs
// Green Parrot Windows
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System;
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
        /// <returns>
        /// The resulting value.
        /// </returns>
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
        public static double Interpolate(double fraction, double l, double r)
        {
            return ((r - l) * fraction) + l;
        }

        /// <summary>
        /// Determines if two points are about equal.
        /// </summary>
        /// <param name="l">The first point.</param>
        /// <param name="r">The second point.</param>
        /// <returns>
        /// True, if two point values are more or less equal.
        /// </returns>
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
        public static bool AboutEqual(double x, double y)
        {
            double epsilon = Math.Max(Math.Abs(x), Math.Abs(y)) * 1E-15;

            return Math.Abs(x - y) <= epsilon;
        }
    }
}
