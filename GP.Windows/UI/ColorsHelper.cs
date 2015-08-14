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
using Windows.UI;

namespace GP.Windows.UI
{
    /// <summary>
    /// Provides some helper methods for dealing with colors.
    /// </summary>
    public static class ColorsHelper
    {
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
        /// Converts the integer to a string, not taking int account the alpha value.
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
        public static Color ConvertToColor(int intColor, double offsetH, double offsetS, double offsetV)
        {
            int integer = intColor;

            byte b = (byte)(integer & 0xFF);
            byte g = (byte)((integer >> 8) & 0xFF);
            byte r = (byte)((integer >> 16) & 0xFF);
            byte a = 0xFF;

            Color color = Color.FromArgb(a, r, g, b);

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

        private static void ColorToHSV(Color color, out double hue, out double saturation, out double value)
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

        private static Color ColorFromHSV(double hue, double saturation, double value)
        {
            int hi = (int)Math.Floor(hue / 60) % 6;

            double f = (hue / 60) - Math.Floor(hue / 60);

            value = value * 255;

            byte v = (byte)value;
            byte p = (byte)(value * (1 - saturation));
            byte q = (byte)(value * (1 - (f * saturation)));
            byte t = (byte)(value * (1 - ((1 - f) * saturation)));
            byte a = 255;

            switch (hi)
            {
                case 0:
                    return Color.FromArgb(a, v, t, p);
                case 1:
                    return Color.FromArgb(a, q, v, p);
                case 2:
                    return Color.FromArgb(a, p, v, t);
                case 3:
                    return Color.FromArgb(a, p, q, v);
                case 4:
                    return Color.FromArgb(a, t, p, v);
                default:
                    return Color.FromArgb(a, v, p, q);
            }
        }
    }
}
