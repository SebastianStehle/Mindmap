// ==========================================================================
// Win2DRenderer.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================
using System;
using System.Collections.Generic;
using GP.Windows;
using System.Linq;
using System.Numerics;
using GP.Windows.UI;
using Hercules.Model.Layouting;
using Hercules.Model.Utils;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Text;

// ReSharper disable UnusedParameter.Local
// ReSharper disable DoNotCallOverridableMethodsInConstructor

namespace Hercules.Model.Rendering.Win2D
{
    public sealed class Win2DRendererBase : DisposableObject, IRenderer
    {
        private readonly Dictionary<NodeBase, Win2DRenderNode> renderNodes = new Dictionary<NodeBase, Win2DRenderNode>();
        private readonly Win2DResourceManager resources;
        private readonly Document document;
        private readonly ThemeBase theme;
        private readonly Func<CanvasDevice> device;
        private readonly ILayout layout;
        private Matrix3x2 transform = Matrix3x2.Identity;
        private Matrix3x2 scale = Matrix3x2.Identity;
        private Matrix3x2 inverseTransform = Matrix3x2.Identity;
        private Matrix3x2 inverseScale = Matrix3x2.Identity;
        private Rect2 visibleRect = new Rect2(0, 0, float.PositiveInfinity, float.PositiveInfinity);
        private float zoomFactor;
        
        public float ZoomFactor
        {
            get
            {
                return zoomFactor;
            }
        }

        public Win2DResourceManager Resources
        {
            get
            {
                return resources;
            }
        }

        public ThemeBase Theme
        {
            get
            {
                return theme;
            }
        }

        public ILayout Layout
        {
            get
            {
                return layout;
            }
        }

        public ICollection<Win2DRenderNode> RenderNodes
        {
            get
            {
                return renderNodes.Values;
            }
        }

        public ICollection<Win2DRenderNode> AllNodes
        {
            get
            {
                return renderNodes.Values.Union(AdditionalNodes).ToList();
            }
        }

        protected IEnumerable<Win2DRenderNode> AdditionalNodes
        {
            get
            {
                yield break;
            }
        }

        public Win2DRendererBase(CanvasDevice device, Document document, ThemeBase theme, ILayout layout)
        {
            Guard.NotNull(layout, nameof(layout));
            Guard.NotNull(theme, nameof(theme));
            Guard.NotNull(device, nameof(device));
            Guard.NotNull(document, nameof(document));

            this.layout = layout;
            this.theme = theme;
            this.device = device;
            this.document = document;
        }

        protected override void DisposeObject(bool disposing)
        {
            document.NodeRemoved -= Document_NodeRemoved;
            document.NodeAdded -= Document_NodeAdded;
        }

        private void InitializeDocument(Document document)
        {
            document.NodeRemoved += Document_NodeRemoved;
            document.NodeAdded += Document_NodeAdded;

            foreach (NodeBase node in document.Nodes)
            {
                TryAdd(node);
            }
        }

        public void Transform(Vector2 translate, float zoom, Rect2 rect)
        {
            visibleRect = rect;
            
            scale = Matrix3x2.CreateScale(zoom);

            transform =
                Matrix3x2.CreateTranslation(
                    translate.X,
                    translate.Y) *
                Matrix3x2.CreateScale(zoom);

            inverseScale = Matrix3x2.CreateScale(1f / zoom);

            inverseTransform =
                Matrix3x2.CreateScale(1f / zoom) *
                Matrix3x2.CreateTranslation(
                    -translate.X,
                    -translate.Y);

            zoomFactor = zoom;
        }

        public bool Render(CanvasDrawingSession session, bool animate, bool invalidateLayout)
        {
            Guard.NotNull(session, nameof(session));

            bool needsRendering = true;

            if (document != null && layout != null)
            {
                session.Transform = transform;
                
                UpdateLayout(session, invalidateLayout);
                UpdateArrangement(session, animate, out needsRendering);

                RenderIt(session);
            }

            return needsRendering;
        }

        private void UpdateLayout(CanvasDrawingSession session, bool invalidateLayout)
        {
            if (invalidateLayout)
            {
                layout.UpdateVisibility(document, this);
            }

            foreach (Win2DRenderNode node in AllNodes)
            {
                node.Measure(session);
            }

            if (invalidateLayout)
            {
                layout.UpdateLayout(document, this);
            }
        }

        private void UpdateArrangement(CanvasDrawingSession session, bool animate, out bool needsRendering)
        {
            needsRendering = false;

            DateTime utcNow = DateTime.UtcNow;

            foreach (Win2DRenderNode node in AllNodes)
            {
                needsRendering |= node.AnimateRenderPosition(animate, utcNow, 600);
            }

            foreach (Win2DRenderNode node in AllNodes)
            {
                node.Arrange(session);
            }
        }

        private void RenderIt(CanvasDrawingSession session)
        {
            session.TextAntialiasing = CanvasTextAntialiasing.Grayscale;
            
            foreach (Win2DRenderNode node in AllNodes)
            {
                if (node.IsVisible && CanRenderPath(node))
                {
                    node.RenderPath(session);
                }
            }

            foreach (Win2DRenderNode node in AllNodes)
            {
                if (node.IsVisible && CanRenderNode(node))
                {
                    node.Render(session);
                }
            }
        }

        private bool CanRenderPath(Win2DRenderNode node)
        {
            return visibleRect.IntersectsWith(node.BoundsWithParent);
        }

        private bool CanRenderNode(Win2DRenderNode node)
        {
            return visibleRect.IntersectsWith(node.Bounds);
        }

        public Vector2 GetMindmapSize(Vector2 position)
        {
            return MathHelper.Transform(position, inverseScale);
        }

        public Vector2 GetMindmapPosition(Vector2 position)
        {
            return MathHelper.Transform(position, inverseTransform);
        }

        public Vector2 GetOverlaySize(Vector2 position)
        {
            return MathHelper.Transform(position, scale);
        }

        public Vector2 GetOverlayPosition(Vector2 position)
        {
            return MathHelper.Transform(position, transform);
        }

        private void Document_NodeAdded(object sender, NodeEventArgs e)
        {
            TryAdd(e.Node);
        }

        private void Document_NodeRemoved(object sender, NodeEventArgs e)
        {
            TryRemove(e.Node);
        }

        private void TryRemove(NodeBase node)
        {
            if (node != null)
            {
                renderNodes.Remove(node);
            }
        }

        private Win2DRenderNode TryAdd(NodeBase node)
        {
            if (node != null)
            {
                return renderNodes.GetOrCreateDefault(node, () =>
                {
                    Win2DRenderNode renderNode = theme.CreateRenderNode(node);

                    renderNode.Parent = TryAdd(node.Parent);

                    return renderNode;
                });
            }

            return null;
        }

        IRenderNode IRenderer.FindRenderNode(NodeBase node)
        {
            return TryAdd(node);
        }
    }
}
