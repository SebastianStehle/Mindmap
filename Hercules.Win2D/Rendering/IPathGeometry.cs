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
        void Arrange(Win2DRenderNode renderNode, CanvasDrawingSession session);

        void Render(Win2DRenderNode win2DRenderNode, CanvasDrawingSession session);
    }
}