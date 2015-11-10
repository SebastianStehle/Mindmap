// ==========================================================================
// ModernPastelPreviewNode.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System;
using System.Numerics;
using Hercules.Model;
using Hercules.Model.Rendering;
using Hercules.Win2D.Rendering.Geometries;
using Hercules.Win2D.Rendering.Utils;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Brushes;
using Microsoft.Graphics.Canvas.Geometry;

namespace Hercules.Win2D.Rendering.Themes.ModernPastel
{
    public sealed class ModernPastelPreviewNode : RenderNodeBase
    {
        private static readonly Vector2 Size = new Vector2(100, 16);
        private CanvasGeometry pathGeometry;

        public override Win2DTextRenderer TextRenderer
        {
            get { throw new NotSupportedException(); }
        }

        public ModernPastelPreviewNode(NodeBase node, Win2DRenderer renderer)
            : base(node, renderer)
        {
        }

        protected override Vector2 MeasureInternal(CanvasDrawingSession session)
        {
            return Size;
        }

        public override void ClearResources()
        {
            base.ClearResources();

            ClearPath();
        }

        private void ClearPath()
        {
            if (pathGeometry != null)
            {
                pathGeometry.Dispose();
                pathGeometry = null;
            }
        }

        public override void ComputePath(CanvasDrawingSession session)
        {
            if (Parent != null)
            {
                ClearPath();

                if (Parent.Node is RootNode)
                {
                    pathGeometry = GeometryBuilder.ComputeFilledPath(this, Parent, session);
                }
                else
                {
                    pathGeometry = GeometryBuilder.ComputeLinePath(this, Parent, session);
                }
            }
        }

        protected override void RenderPathInternal(CanvasDrawingSession session)
        {
            ICanvasBrush brush = Resources.Brush(PathColor, 0.5f);

            if (pathGeometry != null)
            {
                if (Parent.Node is RootNode)
                {
                    session.FillGeometry(pathGeometry, brush);
                }
                else
                {
                    session.DrawGeometry(pathGeometry, brush, 2);
                }
            }
        }

        protected override void RenderInternal(CanvasDrawingSession session, LayoutThemeColor color, bool renderControls)
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
