// ==========================================================================
// Win2DColor.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using Windows.UI;
using GP.Windows.UI;
using Hercules.Model;
using Hercules.Model.Rendering;

namespace Hercules.Win2D.Rendering
{
    public sealed class Win2DColor : IRenderColor
    {
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

        public Win2DColor(Color normal, Color darker, Color lighter)
        {
            this.normal = normal;
            this.darker = darker;
            this.lighter = lighter;
        }

        public Win2DColor(ValueColor color)
            : this(color.Color)
        {
        }

        public Win2DColor(Color color)
            : this(color,
                ColorsHelper.AdjustColor(color, 0, 0.2, -0.3),
                ColorsHelper.AdjustColor(color, 0, -0.2, 0.2))
        {
        }

        public Win2DColor(int color)
            : this(
                ColorsHelper.ConvertToColor(color, 0, 0, 0),
                ColorsHelper.ConvertToColor(color, 0, 0.2, -0.3),
                ColorsHelper.ConvertToColor(color, 0, -0.2, 0.2))
        {
        }
    }
}
