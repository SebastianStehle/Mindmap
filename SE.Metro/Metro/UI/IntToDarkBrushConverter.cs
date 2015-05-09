// ==========================================================================
// IntToDarkBrushConverter.cs
// Metro Library SE
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

namespace SE.Metro.UI
{
    /// <summary>
    /// Converts an integer to a brush but makes the color a little bit darker by changing the value of the color in the HSV colorspace.
    /// </summary>
    public sealed class IntToDarkBrushConverter : IntToBrushConverter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IntToDarkBrushConverter"/> class.
        /// </summary>
        public IntToDarkBrushConverter()
            : base(0, 0.2, -0.3)
        {
        }
    }
}
