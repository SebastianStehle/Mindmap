// ==========================================================================
// IntToLightBrushConverter.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================
namespace GP.Utils.UI
{
    /// <summary>
    /// Converts an integer to a brush but makes the color a little bit lighter by changing the value of the color in the HSV colorspace.
    /// </summary>
    public sealed class IntToLightBrushConverter : IntToBrushConverter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IntToLightBrushConverter"/> class.
        /// </summary>
        public IntToLightBrushConverter()
            : base(0, -0.2, 0.2)
        {
        }
    }
}
