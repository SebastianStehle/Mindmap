// ==========================================================================
// IHullGeometry.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using Microsoft.Graphics.Canvas;

namespace Hercules.Win2D.Rendering
{
    public interface IHullGeometry : IResourceHolder
    {
        void Arrange(Win2DRenderable renderable, CanvasDrawingSession session);

        void Render(Win2DRenderable renderable, CanvasDrawingSession session, Win2DColor color);
    }
}