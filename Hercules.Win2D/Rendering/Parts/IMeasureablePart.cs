// ==========================================================================
// IMeasureablePart.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System.Numerics;
using Microsoft.Graphics.Canvas;

namespace Hercules.Win2D.Rendering.Parts
{
    public interface IMeasureablePart
    {
        Vector2 Measure(Win2DRenderable renderable, ICanvasResourceCreator resourceCreator);
    }
}
