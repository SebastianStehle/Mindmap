// ==========================================================================
// Win2DColor.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System.Numerics;
using Windows.UI;
using GP.Utils.Mathematics;
using GP.Utils.UI;
using Hercules.Model;
using Hercules.Model.Rendering;

namespace Hercules.Win2D.Rendering
{
    public sealed class Win2DColor : IRenderColor
    {
        private readonly Color normalValue;
        private readonly Color darkerValue;
        private readonly Color lighterValue;
        private readonly Vector3 normalVector;
        private readonly Vector3 darkerVector;
        private readonly Vector3 lighterVector;

        public Color Normal
        {
            get { return normalValue; }
        }

        public Color Darker
        {
            get { return darkerValue; }
        }

        public Color Lighter
        {
            get { return lighterValue; }
        }

        Vector3 IRenderColor.Normal
        {
            get { return normalVector; }
        }

        Vector3 IRenderColor.Darker
        {
            get { return darkerVector; }
        }

        Vector3 IRenderColor.Lighter
        {
            get { return lighterVector; }
        }

        public Win2DColor(Color normal, Color darker, Color lighter)
        {
            normalValue = normal;
            normalVector = normal.ToVector3();

            darkerValue = darker;
            darkerVector = darker.ToVector3();

            lighterValue = lighter;
            lighterVector = lighter.ToVector3();
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
