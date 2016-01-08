// ==========================================================================
// IPathGeometry.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using Microsoft.Graphics.Canvas;

namespace Hercules.Win2D.Rendering
{
    public interface IPathGeometry : IResourceHolder
    {
        void Arrange(Win2DRenderable renderable, ICanvasResourceCreator resourceCreator);

        void Render(Win2DRenderable renderable, CanvasDrawingSession session);
    }
}