// ==========================================================================
// Rectangle.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================
namespace Hercules.Win2D.Rendering.Parts.Bodies
{
    public sealed class Rectangle : RectangleBase
    {
        public Rectangle()
            : base(0)
        {
        }

        public override IBodyPart Clone()
        {
            return new Rectangle();
        }
    }
}
