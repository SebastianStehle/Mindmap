// ==========================================================================
// ModernPastelPreviewNode.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using Windows.UI;
using Hercules.Model;
using Hercules.Win2D.Rendering.Geometries.Hulls;
using Hercules.Win2D.Rendering.Geometries.Paths;
using Microsoft.Graphics.Canvas;
using Hercules.Win2D.Rendering.Geometries.Bodies;

namespace Hercules.Win2D.Rendering.Themes.ModernPastel
{
    public sealed class ModernPastelRenderNode : Win2DRenderNode
    {
        public ModernPastelRenderNode(NodeBase node, Win2DRenderer renderer)
            : base(node, renderer)
        {
        }

        protected override IBodyGeometry CreateBody(CanvasDrawingSession session, IBodyGeometry current)
        {
            if (Node is RootNode)
            {
                TextRenderer.FontSize = 16;
            }

            NodeShape nodeShape = CreateShapeFromNode(Node);

            if (current == null)
            {
                return CreateBody(nodeShape);
            }

            NodeShape geometryShape = CreateShapeFromGeometry(current);

            return geometryShape != nodeShape ? CreateBody(nodeShape) : null;
        }

        private static IBodyGeometry CreateBody(NodeShape shape)
        {
            IBodyGeometry result;

            if (shape == NodeShape.Ellipse)
            {
                result = new Ellipse();
            }
            else if (shape == NodeShape.RoundedRectangle)
            {
                result = new RoundedRectangle();
            }
            else if (shape == NodeShape.Rectangle)
            {
                result = new Rectangle();
            }
            else
            {
                result = new Border(Colors.Black);
            }

            return result;
        }

        private static NodeShape CreateShapeFromGeometry(IBodyGeometry current)
        {
            if (current is Ellipse)
            {
                return NodeShape.Ellipse;
            }
            if (current is Rectangle)
            {
                return NodeShape.Rectangle;
            }
            if (current is RoundedRectangle)
            {
                return NodeShape.RoundedRectangle;
            }

            return NodeShape.Border;
        }

        private static NodeShape CreateShapeFromNode(NodeBase node)
        {
            NodeShape shape;

            if (node is RootNode)
            {
                shape = NodeShape.Ellipse;
            }
            else
            {
                Node nodeInstance = (Node)node;

                if (nodeInstance.Shape.HasValue)
                {
                    shape = nodeInstance.Shape.Value;
                }
                else if (node.Parent is RootNode)
                {
                    shape = NodeShape.RoundedRectangle;
                }
                else
                {
                    shape = NodeShape.Border;
                }
            }

            return shape;
        }

        protected override IHullGeometry CreateHull(CanvasDrawingSession session, IHullGeometry current)
        {
            if (current == null && Node.IsShowingHull)
            {
                return new RoundedPolygonHull();
            }

            return null;
        }

        protected override IPathGeometry CreatePath(CanvasDrawingSession session, IPathGeometry current)
        {
            NodeBase parentNode = Parent?.Node;

            if (parentNode != null)
            {
                if (current == null || (current is FilledPath && !(parentNode is RootNode) || current is LinePath && parentNode is RootNode))
                {
                    if (Parent.Node is RootNode)
                    {
                        return new FilledPath(Colors.Black);
                    }

                    return new LinePath(Colors.Black);
                }
            }

            return null;
        }
    }
}
