// ==========================================================================
// IBodyPart.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using GP.Utils;

namespace Hercules.Win2D.Rendering.Parts
{
    public interface IBodyPart : IGeometry, ICloneable<IBodyPart>, IMeasureablePart, IClickablePart
    {
        Win2DTextRenderer TextRenderer { get; }

        float VerticalPathOffset { get; }
    }
}
