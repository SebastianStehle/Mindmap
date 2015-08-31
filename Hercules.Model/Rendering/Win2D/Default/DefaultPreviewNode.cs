// ==========================================================================
// DefaultPreviewNode.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System;
using System.Numerics;
using Hercules.Model.Layouting;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Brushes;

namespace Hercules.Model.Rendering.Win2D.Default
{
    public sealed class DefaultPreviewNode : DefaultRenderNode
    {
        private static readonly Vector2 Size = new Vector2(100, 16);

        public override Win2DTextRenderer TextRenderer
        {
            get { throw new NotSupportedException(); }
        }

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
            ICanvasBrush brush = Resources.Brush(PathColor, 0.5f);

            if (Parent != null)
            {
                if (Parent.Node is RootNode)
                {
                    PathRenderer.RenderFilledPath(this, Parent, session, brush);
                }
                else
                {
                    PathRenderer.RenderLinePath(this, Parent, session, brush);
                }
            }
        }

        protected override void RenderInternal(CanvasDrawingSession session, ThemeColor color, bool renderControls)
        {
            if (Parent != null)
            {
                session.FillRoundedRectangle(Bounds, 2, 2, Resources.Brush(PathColor, 0.5f));
            }
        }

        protected override Win2DRenderNode CloneInternal()
        {
            throw new NotSupportedException();
        }
    }
}
