// ==========================================================================
// RenderThemeColor.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using Windows.UI;
using GP.Windows.UI;

namespace Hercules.Model.Rendering
{
    public sealed class LayoutThemeColor
    {
        public static readonly LayoutThemeColor White = new LayoutThemeColor(Colors.White, Colors.White, Colors.White);
        private readonly Color normal;
        private readonly Color darker;
        private readonly Color lighter;

        public Color Normal
        {
            get { return normal; }
        }

        public Color Darker
        {
            get { return darker; }
        }

        public Color Lighter
        {
            get { return lighter; }
        }

        public LayoutThemeColor(Color normal, Color darker, Color lighter)
        {
            this.darker = darker;

            this.normal = normal;

            this.lighter = lighter;
        }

        public LayoutThemeColor(CustomColor color)
            : this(color.Color)
        {
        }

        public LayoutThemeColor(Color color)
            : this(color,
                ColorsHelper.AdjustColor(color, 0, 0.2, -0.3),
                ColorsHelper.AdjustColor(color, 0, -0.2, 0.2))
        {
        }

        public LayoutThemeColor(int color)
            : this(
                ColorsHelper.ConvertToColor(color, 0, 0, 0),
                ColorsHelper.ConvertToColor(color, 0, 0.2, -0.3),
                ColorsHelper.ConvertToColor(color, 0, -0.2, 0.2))
        {
        }
    }
}
