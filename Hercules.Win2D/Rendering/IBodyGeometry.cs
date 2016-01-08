// ==========================================================================
// IBodyGeometry.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System.Numerics;
using Microsoft.Graphics.Canvas;

namespace Hercules.Win2D.Rendering
{
    public interface IBodyGeometry : IResourceHolder
    {
        Win2DTextRenderer TextRenderer { get; }

        float VerticalPathOffset { get; }

        Vector2 Measure(Win2DRenderable renderable, ICanvasResourceCreator resourceCreator);

        void Arrange(Win2DRenderable renderable, ICanvasResourceCreator resourceCreator);

        void Render(Win2DRenderable renderable, CanvasDrawingSession session, Win2DColor color, bool renderSelection);

        IBodyGeometry Clone();
    }
}
