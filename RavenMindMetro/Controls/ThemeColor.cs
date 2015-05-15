// ==========================================================================
// ThemeColor.cs
// Metro Library SE
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using Windows.UI;
using Windows.UI.Xaml.Media;

namespace RavenMind.Controls
{
    public sealed class ThemeColor
    {
        public static readonly ThemeColor White = new ThemeColor(new SolidColorBrush(Colors.White), new SolidColorBrush(Colors.White), new SolidColorBrush(Colors.White));

        private readonly SolidColorBrush normal;
        private readonly SolidColorBrush dark;
        private readonly SolidColorBrush light;

        public SolidColorBrush Normal
        {
            get
            {
                return normal;
            }
        }

        public SolidColorBrush Dark
        {
            get
            {
                return dark;
            }
        }

        public SolidColorBrush Light
        {
            get
            {
                return light;
            }
        }

        public ThemeColor(SolidColorBrush normal, SolidColorBrush dark, SolidColorBrush light)
        {
            this.dark = dark;
            this.light = light;
            this.normal = normal;
        }
    }
}
