// ==========================================================================
// RoundedRectangle.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================
namespace Hercules.Win2D.Rendering.Parts.Bodies
{
    public sealed class RoundedRectangle : RectangleBase
    {
        public RoundedRectangle()
            : base(10)
        {
        }

        public override IBodyPart Clone()
        {
            return new RoundedRectangle();
        }
    }
}
