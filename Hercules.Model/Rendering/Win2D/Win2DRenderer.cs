// ==========================================================================
// Win2DRenderer.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using Windows.Graphics.Display;
using Windows.Storage.Streams;
using Windows.UI;
using GP.Windows;
using GP.Windows.UI;
using GP.Windows.UI.Controls;
using Hercules.Model.Layouting;
using Hercules.Model.Utils;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Text;
using Microsoft.Graphics.Canvas.UI.Xaml;

// ReSharper disable UnusedParameter.Local
// ReSharper disable DoNotCallOverridableMethodsInConstructor

namespace Hercules.Model.Rendering.Win2D
{
    public abstract class Win2DRenderer : DisposableObject, IRenderer
    {
        private readonly Dictionary<NodeBase, Win2DRenderNode> renderNodes = new Dictionary<NodeBase, Win2DRenderNode>();
        private readonly List<Win2DRenderNode> customNodes = new List<Win2DRenderNode>();
        private readonly Win2DRenderNode previewNode;
        private readonly Win2DResourceManager resources;
        private readonly Document document;
        private readonly ICanvasControl canvas;
        private Matrix3x2 transform = Matrix3x2.Identity;
        private Matrix3x2 scale = Matrix3x2.Identity;
        private Matrix3x2 inverseTransform = Matrix3x2.Identity;
        private Matrix3x2 inverseScale = Matrix3x2.Identity;
        private Rect2 visibleRect = new Rect2(0, 0, float.PositiveInfinity, float.PositiveInfinity);
        private Rect2 sceneBounds;
        private float zoomFactor;
        private ILayout layout;
        private bool layoutInvalidated;

        public float ZoomFactor
        {
            get
            {
                return zoomFactor;
            }
        }

        public ICanvasControl Canvas
        {
            get
            {
                return canvas;
            }
        }

        public Document Document
        {
            get
            {
                return document;
            }
        }

        public Win2DResourceManager Resources
        {
            get
            {
                return resources;
            }
        }

        public ICollection<Win2DRenderNode> RenderNodes
        {
            get
            {
                return renderNodes.Values;
            }
        }

        public ICollection<Win2DRenderNode> CustomNodes
        {
            get
            {
                return customNodes;
            }
        }

        protected ICollection<Win2DRenderNode> AllNodes
        {
            get
            {
                return renderNodes.Values.Union(customNodes).Union(new Win2DRenderNode[] { previewNode }).ToList();
            }
        }

        public ILayout Layout
        {
            get
            {
                return layout;
            }
            set
            {
                Guard.NotNull(value, nameof(value));

                if (layout != value)
                {
                    layout = value;

                    Invalidate();
                }
            }
        }

        protected Win2DRenderer(Document document, ICanvasControl canvas)
        {
            Guard.NotNull(document, nameof(document));
            Guard.NotNull(canvas, nameof(canvas));

            this.canvas = canvas;
            this.document = document;

            InitializeCanvas();
            InitializeDocument();

            previewNode = CreatePreviewNode();

            resources = new Win2DResourceManager(canvas);
        }

        protected override void DisposeObject(bool disposing)
        {
            ReleaseDocument();
            ReleaseCanvas();
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

        private void InitializeDocument()
        {
            document.StateChanged += Document_StateChanged;
            document.NodeRemoved += Document_NodeRemoved;
            document.NodeAdded += Document_NodeAdded;

            foreach (NodeBase node in document.Nodes)
            {
                TryAdd(node);
            }
        }

        private void ReleaseDocument()
        {
            document.StateChanged -= Document_StateChanged;
            document.NodeRemoved -= Document_NodeRemoved;
            document.NodeAdded -= Document_NodeAdded;

            foreach (NodeBase node in document.Nodes)
            {
                TryRemove(node);
            }
        }

        private void InitializeCanvas()
        {
            canvas.Draw += Canvas_Draw;
        }

        private void ReleaseCanvas()
        {
            canvas.Draw -= Canvas_Draw;
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

        public void Invalidate()
        {
            layoutInvalidated = true;

            canvas.Invalidate();
        }

        private void Canvas_Draw(object sender, CanvasDrawEventArgs args)
        {
            Render(args.DrawingSession);
        }

        public async Task RenderScreenshotAsync(IRandomAccessStream stream, Color background, float padding = 20)
        {
            Guard.NotNull(stream, nameof(stream));
            Guard.GreaterThan(padding, 0, nameof(padding));

            float w = sceneBounds.Size.X + 2 * padding;
            float h = sceneBounds.Size.Y + 2 * padding;

            float dpi = DisplayInformation.GetForCurrentView().LogicalDpi;

            using (CanvasRenderTarget target = new CanvasRenderTarget(canvas.Device, w, h, dpi))
            {
                using (CanvasDrawingSession session = target.CreateDrawingSession())
                {
                    session.Clear(background);

                    session.Transform =
                        Matrix3x2.CreateTranslation(
                            -sceneBounds.Position.X + padding,
                            -sceneBounds.Position.Y + padding);

                    RenderIt(session, false, false);
                }

                await target.SaveAsync(stream, CanvasBitmapFileFormat.Png);
            }
        }

        private void Render(CanvasDrawingSession session)
        {
            if (layout != null)
            {
                session.Transform = transform;

                bool needsRedraw;

                UpdateLayout(session);
                UpdateArrangement(session, true, out needsRedraw);

                RenderIt(session, true, true);

                if (needsRedraw)
                {
                    canvas.Invalidate();
                }
            }
        }

        private void UpdateLayout(CanvasDrawingSession session)
        {
            if (layoutInvalidated)
            {
                layout.UpdateVisibility(document, this);
            }

            foreach (Win2DRenderNode node in AllNodes)
            {
                node.Measure(session);
            }

            if (layoutInvalidated)
            {
                layout.UpdateLayout(document, this);

                layoutInvalidated = false;
            }
        }

        private void UpdateArrangement(CanvasDrawingSession session, bool animate, out bool needsRedraw)
        {
            needsRedraw = false;

            DateTime utcNow = DateTime.UtcNow;

            foreach (Win2DRenderNode node in AllNodes)
            {
                bool isCustomNode = IsCustomNode(node);

                needsRedraw |= node.AnimateRenderPosition(animate && !isCustomNode, utcNow, 600);
            }

            double minX = double.MaxValue;
            double minY = double.MaxValue;
            double maxX = double.MinValue;
            double maxY = double.MinValue;

            foreach (Win2DRenderNode node in AllNodes)
            {
                node.Arrange(session);

                if (node.IsVisible && !IsCustomNode(node))
                {
                    Rect2 bounds = node.Bounds;

                    minX = Math.Min(minX, bounds.Left);
                    minY = Math.Min(minY, bounds.Top);
                    maxX = Math.Max(maxX, bounds.Right);
                    maxY = Math.Max(maxY, bounds.Bottom);
                }
            }

            sceneBounds = new Rect2((float)minX, (float)minY, (float)(maxX - minX), (float)(maxY - minY));
        }

        private void RenderIt(CanvasDrawingSession session, bool renderControls, bool renderCustoms)
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
                    bool isCustomNode = IsCustomNode(node);

                    if (renderCustoms || !isCustomNode)
                    {
                        node.Render(session, renderControls && !isCustomNode);
                    }
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

        private bool IsCustomNode(Win2DRenderNode node)
        {
            return customNodes.Contains(node) || node == previewNode;
        }

        public void AddCustomNode(Win2DRenderNode node)
        {
            customNodes.Add(node);
        }

        public void RemoveCustomNode(Win2DRenderNode node)
        {
            customNodes.Remove(node);
        }

        public bool HandleClick(Vector2 hitPosition, out Win2DRenderNode handledNode)
        {
            handledNode = null;

            foreach (Win2DRenderNode renderNode in renderNodes.Values)
            {
                if (renderNode.HandleClick(hitPosition))
                {
                    handledNode = renderNode;

                    Invalidate();
                    
                    return true;
                }
            }

            return false;
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

        private void Document_StateChanged(object sender, EventArgs e)
        {
            Invalidate();
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
                    Win2DRenderNode renderNode = CreateRenderNode(node);

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

        protected abstract Win2DRenderNode CreatePreviewNode();

        protected abstract Win2DRenderNode CreateRenderNode(NodeBase node);
    }
}
