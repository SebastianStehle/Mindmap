// ==========================================================================
// DefaultPreviewNode.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System.Numerics;
using Microsoft.Graphics.Canvas;

namespace Hercules.Model.Rendering.Win2D.Default
{
    public sealed class DefaultPreviewNode : DefaultRootNode
    {
        private static readonly Vector2 Size = new Vector2(100, 16);

        public DefaultPreviewNode(NodeBase node, DefaultRenderer renderer) 
            : base(node, renderer)
        {
        }

        protected override Vector2 MeasureInternal(CanvasDrawingSession session)
        {
            return Size;
        }

        protected override void RenderPathInternal(CanvasDrawingSession session)
        {
            RenderPath(session, Resources.Brush(PathColor, 0.5f));
        }

        protected override void RenderInternal(CanvasDrawingSession session, ThemeColor color, bool renderControls)
        {
            if (Parent != null)
            {
                session.FillRoundedRectangle(Bounds, 2, 2, Resources.Brush(PathColor, 0.5f));
            }
        }
    }
}
