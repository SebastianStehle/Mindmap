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
        Vector2 TextRenderPosition { get; }

        Vector2 RenderPositionOffset { get; }

        bool HasText { get; }

        float VerticalPathOffset { get; }

        void Arrange(Win2DRenderNode renderNode, CanvasDrawingSession session);

        void Render(Win2DRenderNode renderNode, CanvasDrawingSession session, Win2DColor color, bool renderSelection);

        Vector2 Measure(Win2DRenderNode renderNode, CanvasDrawingSession session, Vector2 textSize);
    }
}
