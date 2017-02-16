// ==========================================================================
// ModernPastelRenderNode.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using Windows.UI;
using Hercules.Model;
using Hercules.Win2D.Rendering.Parts;
using Hercules.Win2D.Rendering.Parts.Bodies;
using Hercules.Win2D.Rendering.Parts.Hulls;
using Hercules.Win2D.Rendering.Parts.Paths;

// ReSharper disable InvertIf

namespace Hercules.Win2D.Rendering.Themes.ModernPastel
{
    public sealed class ModernPastelRenderNode : Win2DRenderNode
    {
        public ModernPastelRenderNode(NodeBase node, Win2DRenderer renderer)
            : base(node, renderer)
        {
        }

        protected override IBodyPart CreateBody(IBodyPart current)
        {
            IBodyPart geometry = null;

            var nodeShape = CreateShapeFromNode(Node);

            if (current == null)
            {
                geometry = CreateBody(nodeShape);
            }

            var geometryShape = CreateShapeFromGeometry(current);

            if (geometryShape != nodeShape)
            {
                geometry = CreateBody(nodeShape);
            }

            if (Node is RootNode && geometry != null)
            {
                geometry.TextRenderer.FontSize = 16;
            }

            return geometry;
        }

        private static IBodyPart CreateBody(NodeShape shape)
        {
            IBodyPart result;

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

        private static NodeShape CreateShapeFromGeometry(IBodyPart current)
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
                var nodeInstance = (Node)node;

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

        protected override IHullPart CreateHull(IHullPart current)
        {
            if (current == null && Node.IsShowingHull)
            {
                return new RoundedPolygonHull();
            }

            return null;
        }

        protected override IPathPart CreatePath(IPathPart current)
        {
            var parentNode = Parent?.Node;

            if (parentNode == null)
            {
                return null;
            }

            if (current == null || (current is FilledPath && !(parentNode is RootNode)) || (current is LinePath && parentNode is RootNode))
            {
                if (Parent.Node is RootNode)
                {
                    return new FilledPath(Colors.Black);
                }

                return new LinePath(Colors.Black);
            }

            return null;
        }
    }
}
