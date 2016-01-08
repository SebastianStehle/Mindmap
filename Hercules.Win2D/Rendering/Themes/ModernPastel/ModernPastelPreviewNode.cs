// ==========================================================================
// ModernPastelPreviewNode.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System.Numerics;
using Windows.UI;
using Hercules.Model;
using Hercules.Win2D.Rendering.Geometries.Bodies;
using Hercules.Win2D.Rendering.Geometries.Paths;
using Microsoft.Graphics.Canvas;

namespace Hercules.Win2D.Rendering.Themes.ModernPastel
{
    public sealed class ModernPastelPreviewNode : Win2DRenderNode
    {
        private static readonly Vector2 Size = new Vector2(60, 16);

        public ModernPastelPreviewNode(NodeBase node, Win2DRenderer renderer)
            : base(node, renderer)
        {
        }

        protected override IBodyGeometry CreateBody(ICanvasResourceCreator resourceCreator, IBodyGeometry current)
        {
            return current == null ? new SimpleRectangle(Size) : null;
        }

        protected override IHullGeometry CreateHull(ICanvasResourceCreator resourceCreator, IHullGeometry current)
        {
            return null;
        }

        protected override IPathGeometry CreatePath(ICanvasResourceCreator resourceCreator, IPathGeometry current)
        {
            NodeBase parentNode = Parent?.Node;

            if (parentNode != null)
            {
                if (current == null || (current is FilledPath && !(parentNode is RootNode)) || (current is LinePath && parentNode is RootNode))
                {
                    if (Parent.Node is RootNode)
                    {
                        return new FilledPath(Colors.Black, 0.5f);
                    }

                    return new LinePath(Colors.Black, 0.5f);
                }
            }

            return null;
        }
    }
}
