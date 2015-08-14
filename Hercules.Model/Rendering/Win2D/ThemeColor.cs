// ==========================================================================
// ThemeColor.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================
using Windows.UI;

namespace Hercules.Model.Rendering.Win2D
{
    public sealed class ThemeColor
    {
        public static readonly ThemeColor White = new ThemeColor(Colors.White, Colors.White, Colors.White);
        private readonly Color normal;
        private readonly Color dark;
        private readonly Color light;

        public Color Normal
        {
            get
            {
                return normal;
            }
        }

        public Color Dark
        {
            get
            {
                return dark;
            }
        }

        public Color Light
        {
            get
            {
                return light;
            }
        }

        public ThemeColor(Color normal, Color dark, Color light)
        {
            this.dark = dark;

            this.light = light;

            this.normal = normal;
        }
    }
}
