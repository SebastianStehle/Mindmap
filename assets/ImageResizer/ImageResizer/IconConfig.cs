// ==========================================================================
// IconConfig.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System.Drawing;

namespace ImageResizer
{
    internal sealed class IconConfig
    {
        private readonly string sourceName;
        private readonly string targetName;
        private readonly Size size;
        private readonly double[] scalings = { 1, 1.25, 1.5, 2, 4 };

        public double[] Scalings
        {
            get { return scalings; }
        }

        public string SourceName
        {
            get { return sourceName; }
        }

        public string TargetName
        {
            get { return targetName; }
        }

        public Size Size
        {
            get { return size; }
        }

        public IconConfig(string sourceName, string targetName, Size size)
        {
            this.sourceName = sourceName;
            this.targetName = targetName;

            this.size = size;
        }
    }
}
