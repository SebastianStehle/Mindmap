﻿// ==========================================================================
// Win2DScene.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using GP.Windows;
using Hercules.Model;
using Hercules.Model.Layouting;
using Hercules.Model.Rendering;
using Hercules.Model.Utils;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Text;

// ReSharper disable LoopCanBeConvertedToQuery
// ReSharper disable LoopCanBePartlyConvertedToQuery

namespace Hercules.Win2D.Rendering
{
    public sealed class Win2DScene : DisposableObject, IRenderScene, IResourceHolder
    {
        private readonly Dictionary<NodeBase, Win2DRenderNode> renderNodes = new Dictionary<NodeBase, Win2DRenderNode>();
        private readonly List<Win2DAdornerRenderNode> adorners = new List<Win2DAdornerRenderNode>();
        private readonly Func<NodeBase, Win2DRenderNode> nodeFactory;
        private readonly Win2DRenderNode previewNode;
        private readonly Document document;
        private Rect2 bounds;
        private bool isLayoutInvalidated;

        public Rect2 Bounds
        {
            get { return bounds; }
        }

        public ICollection<Win2DRenderNode> DiagramNodes
        {
            get { return renderNodes.Values; }
        }

        public ICollection<Win2DRenderNode> NonDiagramNodes
        {
            get { return new[] { previewNode }; }
        }

        public ICollection<Win2DRenderNode> AllNodes
        {
            get { return new[] { previewNode }.Union(renderNodes.Values).ToList(); }
        }

        internal Win2DScene(Document document, Win2DRenderNode previewNode, Func<NodeBase, Win2DRenderNode> nodeFactory)
        {
            this.document = document;
            this.previewNode = previewNode;
            this.nodeFactory = nodeFactory;

            InitializeDocument();
        }

        protected override void DisposeObject(bool disposing)
        {
            ReleaseDocument();
        }

        public void ClearResources()
        {
            foreach (Win2DRenderNode renderNode in AllNodes)
            {
                renderNode.ClearResources();
            }

            foreach (Win2DAdornerRenderNode adorner in adorners)
            {
                adorner.ClearResources();
            }
        }

        private void InitializeDocument()
        {
            document.NodeRemoved += Document_NodeRemoved;
            document.NodeAdded += Document_NodeAdded;

            foreach (NodeBase node in document.Nodes)
            {
                TryAdd(node);
            }
        }

        private void ReleaseDocument()
        {
            document.NodeRemoved -= Document_NodeRemoved;
            document.NodeAdded -= Document_NodeAdded;

            foreach (NodeBase node in document.Nodes)
            {
                TryRemove(node);
            }
        }

        public void InvalidateLayout()
        {
            isLayoutInvalidated = true;
        }

        public void HidePreviewElement()
        {
            previewNode.Hide();
        }

        public void ShowPreviewElement(Vector2 position, NodeBase parent, AnchorPoint anchor)
        {
            Win2DRenderNode parentNode = TryAdd(parent);

            previewNode.MoveToLayout(position, anchor);
            previewNode.Parent = parentNode;
            previewNode.Show();
        }

        public void UpdateLayout(CanvasDrawingSession session, ILayout layout)
        {
            foreach (Win2DRenderNode renderNode in AllNodes)
            {
                renderNode.ComputeBody(session);
                renderNode.ComputeHull(session);
                renderNode.ComputePath(session);
            }

            if (isLayoutInvalidated)
            {
                layout.UpdateVisibility(document, this);
            }

            foreach (Win2DRenderNode renderNode in AllNodes)
            {
                renderNode.Measure(session);
            }

            foreach (Win2DAdornerRenderNode adorner in adorners)
            {
                adorner.Measure(session);
            }

            if (isLayoutInvalidated)
            {
                layout.UpdateLayout(document, this);

                foreach (Win2DRenderNode renderNode in renderNodes.Values)
                {
                    if (renderNode.IsVisible)
                    {
                        renderNode.ComputeHull(session);
                    }
                }

                isLayoutInvalidated = false;
            }
        }

        public void UpdateArrangement(CanvasDrawingSession session, bool animate, out bool needsRedraw)
        {
            needsRedraw = false;

            DateTime utcNow = DateTime.UtcNow;

            foreach (Win2DRenderNode renderNode in AllNodes)
            {
                needsRedraw |= renderNode.AnimateRenderPosition(animate, utcNow, 600);
            }

            double minX = double.MaxValue;
            double minY = double.MaxValue;
            double maxX = double.MinValue;
            double maxY = double.MinValue;

            foreach (Win2DRenderNode renderNode in AllNodes)
            {
                renderNode.Arrange(session);

                if (renderNode.IsVisible)
                {
                    Rect2 nodeBounds = renderNode.RenderBounds;

                    minX = Math.Min(minX, nodeBounds.Left);
                    minY = Math.Min(minY, nodeBounds.Top);
                    maxX = Math.Max(maxX, nodeBounds.Right);
                    maxY = Math.Max(maxY, nodeBounds.Bottom);
                }
            }

            foreach (Win2DAdornerRenderNode adorner in adorners)
            {
                adorner.Arrange(session);
            }

            bounds = new Rect2((float)minX, (float)minY, (float)(maxX - minX), (float)(maxY - minY));
        }

        public void Render(CanvasDrawingSession session, bool renderControls, Rect2 viewRect)
        {
            session.TextAntialiasing = CanvasTextAntialiasing.Grayscale;

            foreach (Win2DRenderNode renderNode in DiagramNodes)
            {
                if (renderNode.Node.HasChildren && renderNode.IsVisible && !renderNode.Node.IsCollapsed && renderNode.Node.IsShowingHull)
                {
                    renderNode.RenderHull(session);
                }
            }

            foreach (Win2DRenderNode renderNode in AllNodes)
            {
                if (renderNode.IsVisible && CanRenderPath(renderNode, viewRect))
                {
                    renderNode.RenderPath(session);
                }
            }

            foreach (Win2DRenderNode renderNode in AllNodes)
            {
                if (renderNode.IsVisible && CanRenderNode(renderNode, viewRect))
                {
                    renderNode.Render(session, renderControls);
                }
            }

            if (renderControls)
            {
                foreach (Win2DAdornerRenderNode adorner in adorners)
                {
                    adorner.Render(session);
                }
            }
        }

        public bool HandleClick(Vector2 hitPosition, out Win2DRenderNode handledNode)
        {
            handledNode = null;

            foreach (Win2DRenderNode renderNode in DiagramNodes)
            {
                if (renderNode.HandleClick(hitPosition))
                {
                    handledNode = renderNode;

                    return true;
                }
            }

            return false;
        }

        private static bool CanRenderPath(Win2DRenderNode renderNode, Rect2 viewRect)
        {
            return viewRect.IntersectsWith(renderNode.RenderBoundsWithParent);
        }

        private static bool CanRenderNode(IRenderable renderNode, Rect2 viewRect)
        {
            return viewRect.IntersectsWith(renderNode.RenderBounds);
        }

        private void Document_NodeAdded(object sender, NodeEventArgs e)
        {
            TryAdd(e.Node);
        }

        private void Document_NodeRemoved(object sender, NodeEventArgs e)
        {
            TryRemove(e.Node);
        }

        public void RemoveAdorner(IAdornerRenderNode adorner)
        {
            Guard.NotNull(adorner, nameof(adorner));

            adorners.Remove((Win2DAdornerRenderNode)adorner);
        }

        public IAdornerRenderNode CreateAdorner(IRenderNode renderNode)
        {
            Guard.NotNull(renderNode, nameof(renderNode));

            return adorners.AddAndReturn(((Win2DRenderNode)renderNode).CreateAdorner());
        }

        private void TryRemove(NodeBase node)
        {
            if (node != null)
            {
                Win2DRenderNode oldRenderNode;

                if (renderNodes.TryGetValue(node, out oldRenderNode))
                {
                    oldRenderNode.ClearResources();

                    renderNodes.Remove(node);
                }
            }
        }

        private Win2DRenderNode TryAdd(NodeBase node)
        {
            if (node != null)
            {
                return renderNodes.GetOrCreateDefault(node, () =>
                {
                    Win2DRenderNode renderNode = nodeFactory(node);

                    renderNode.Parent = TryAdd(node.Parent);

                    return renderNode;
                });
            }

            return null;
        }

        public IRenderNode FindRenderNode(NodeBase node)
        {
            return TryAdd(node);
        }
    }
}
