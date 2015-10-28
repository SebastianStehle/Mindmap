// ==========================================================================
// ColorsHelper.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System;
using System.Globalization;
using System.Text;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace GP.Windows.UI
{
    /// <summary>
    /// Provides some helper methods for dealing with colors.
    /// </summary>
    public static class ColorsHelper
    {
        private static readonly Color[] ColorGradients =
        {
            Color.FromArgb(255, 255, 0, 0),
            Color.FromArgb(255, 255, 255, 0),
            Color.FromArgb(255, 0, 255, 0),
            Color.FromArgb(255, 0, 255, 255),
            Color.FromArgb(255, 0, 0, 255),
            Color.FromArgb(255, 255, 0, 255)
        };

        /// <summary>
        /// Gets a brush with all colors.
        /// </summary>
        /// <param name="orientation">The orientation of the brush.</param>
        /// <returns>
        /// The generated brush.
        /// </returns>
        public static LinearGradientBrush GetColorGradientBrush(Orientation orientation)
        {
            return CreateGradientBrush(orientation, ColorGradients);
        }

        private static LinearGradientBrush CreateGradientBrush(Orientation orientation, params Color[] colors)
        {
            LinearGradientBrush brush = new LinearGradientBrush();

            float negatedStops = 1 / (float)colors.Length;

            for (var i = 0; i < colors.Length; i++)
            {
                brush.GradientStops.Add(new GradientStop { Offset = negatedStops * i, Color = colors[i] });
            }

            brush.GradientStops.Add(new GradientStop { Offset = negatedStops * colors.Length, Color = colors[0] });

            if (orientation == Orientation.Vertical)
            {
                brush.StartPoint = new Point(0, 1);

                brush.EndPoint = new Point();
            }
            else
            {
                brush.EndPoint = new Point(1, 0);
            }

            return brush;
        }

        /// <summary>
        /// Converts the integer to a string, not taking int account the alpha value.
        /// </summary>
        /// <param name="intColor">The color value.</param>
        /// <returns>The resulting string.</returns>
        public static string ConvertToRGBString(int intColor)
        {
            return ConvertToRGBString(intColor, 0, 0, 0);
        }

        /// <summary>
        /// Converts the integer to a string, not taking int account the alpha value using the offset for hue, saturation and value.
        /// </summary>
        /// <param name="offsetH">The hue offset.</param>
        /// <param name="offsetS">The saturation offset.</param>
        /// <param name="offsetV">The value offset.</param>
        /// <param name="intColor">The color value.</param>
        /// <returns>The resulting string.</returns>
        public static string ConvertToRGBString(int intColor, double offsetH, double offsetS, double offsetV)
        {
            return ConvertToRGBString(ConvertToColor(intColor, offsetH, offsetS, offsetV));
        }

        /// <summary>
        /// Converts the color to a string, not taking int account the alpha value.
        /// </summary>
        /// <param name="color">The color value.</param>
        /// <returns>The resulting string.</returns>
        public static string ConvertToRGBString(Color color)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("#");
            stringBuilder.AppendFormat(CultureInfo.InvariantCulture, "{0:X2}", color.R);
            stringBuilder.AppendFormat(CultureInfo.InvariantCulture, "{0:X2}", color.G);
            stringBuilder.AppendFormat(CultureInfo.InvariantCulture, "{0:X2}", color.B);

            return stringBuilder.ToString();
        }

        /// <summary>
        /// Converts the color to a integer, not taking int account the alpha value.
        /// </summary>
        /// <param name="color">The color value.</param>
        /// <returns>The resulting integer.</returns>
        public static int ConvertToInt(Color color)
        {
            return color.R << 16 | color.G << 8 | color.B;
        }

        /// <summary>
        /// Converts the integer to a color object, not taking int account the alpha value.
        /// </summary>
        /// <param name="intColor">The color value.</param>
        /// <returns>The resulting color object.</returns>
        public static Color ConvertToColor(int intColor)
        {
            return ConvertToColor(intColor, 0, 0, 0);
        }

        /// <summary>
        /// Converts the integer to a color object, not taking int account the alpha value using the offset for hue, saturation and value.
        /// </summary>
        /// <param name="offsetH">The hue offset.</param>
        /// <param name="offsetS">The saturation offset.</param>
        /// <param name="offsetV">The value offset.</param>
        /// <param name="intColor">The color value.</param>
        /// <returns>The resulting color object.</returns>
        public static int AdjustColor(int intColor, double offsetH, double offsetS, double offsetV)
        {
            Color color = ConvertToColor(intColor, offsetH, offsetS, offsetV);

            return ConvertToInt(color);
        }

        /// <summary>
        /// Adjust the colorvalue using the offset for hue, saturation and value.
        /// </summary>
        /// <param name="offsetH">The hue offset.</param>
        /// <param name="offsetS">The saturation offset.</param>
        /// <param name="offsetV">The value offset.</param>
        /// <param name="color">The color value.</param>
        /// <returns>The resulting color object.</returns>
        public static Color AdjustColor(Color color, double offsetH, double offsetS, double offsetV)
        {
            double h;
            double s;
            double v;

            ColorToHSV(color, out h, out s, out v);

            v = Math.Max(0, Math.Min(1, v + offsetV));
            s = Math.Max(0, Math.Min(1, s + offsetS));

            h = (h + offsetH) % 360;

            color = ColorFromHSV(h, s, v);

            return color;
        }

        /// <summary>
        /// Converts the integer to a color object, not taking int account the alpha value using the offset for hue, saturation and value.
        /// </summary>
        /// <param name="offsetH">The hue offset.</param>
        /// <param name="offsetS">The saturation offset.</param>
        /// <param name="offsetV">The value offset.</param>
        /// <param name="intColor">The color value.</param>
        /// <returns>The resulting color object.</returns>
        public static Color ConvertToColor(int intColor, double offsetH, double offsetS, double offsetV)
        {
            int integer = intColor;

            byte b = (byte)(integer & 0xFF);
            byte g = (byte)((integer >> 8) & 0xFF);
            byte r = (byte)((integer >> 16) & 0xFF);

            Color color = Color.FromArgb(0xFF, r, g, b);

            return AdjustColor(color, offsetH, offsetS, offsetV);
        }

        /// <summary>
        /// Converts the color to HSV color space.
        /// </summary>
        /// <param name="color">The color to convert.</param>
        /// <param name="hue">The resulting hue number.</param>
        /// <param name="saturation">The resulting saturation number.</param>
        /// <param name="value">The resulting value number.</param>
        public static void ColorToHSV(Color color, out double hue, out double saturation, out double value)
        {
            double r = color.R / 255d;
            double g = color.G / 255d;
            double b = color.B / 255d;

            double max = Math.Max(r, Math.Max(g, b));
            double min = Math.Min(r, Math.Min(g, b));

            hue = 0;

            if (Math.Abs(max - r) < double.Epsilon)
            {
                if (g > b)
                {
                    hue = 60 * (g - b) / (max - min);
                }
                else if (g < b)
                {
                    hue = (60 * (g - b) / (max - min)) + 360;
                }
            }
            else if (Math.Abs(max - g) < double.Epsilon)
            {
                hue = (60 * (b - r) / (max - min)) + 120;
            }
            else if (Math.Abs(max - b) < double.Epsilon)
            {
                hue = (60 * (r - g) / (max - min)) + 240;
            }

            saturation = (Math.Abs(max) < double.Epsilon) ? 0 : 1 - (min / max);

            value = max;
        }

        /// <summary>
        /// Converts from HSV color space to ARGB.
        /// </summary>
        /// <param name="hue">The hue number.</param>
        /// <param name="saturation">The saturation number.</param>
        /// <param name="value">The value number.</param>
        /// <returns>
        /// The resulting color.
        /// </returns>
        public static Color ColorFromHSV(double hue, double saturation, double value)
        {
            int hi = (int)Math.Floor(hue / 60) % 6;

            double f = (hue / 60) - Math.Floor(hue / 60);

            value = value * 255;

            byte v = (byte)value;
            byte p = (byte)(value * (1 - saturation));
            byte q = (byte)(value * (1 - (f * saturation)));
            byte t = (byte)(value * (1 - ((1 - f) * saturation)));

            switch (hi)
            {
                case 0:
                    return Color.FromArgb(0xFF, v, t, p);
                case 1:
                    return Color.FromArgb(0xFF, q, v, p);
                case 2:
                    return Color.FromArgb(0xFF, p, v, t);
                case 3:
                    return Color.FromArgb(0xFF, p, q, v);
                case 4:
                    return Color.FromArgb(0xFF, t, p, v);
                default:
                    return Color.FromArgb(0xFF, v, p, q);
            }
        }
    }
}
