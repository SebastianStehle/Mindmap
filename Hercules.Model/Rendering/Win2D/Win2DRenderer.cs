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
using Windows.Graphics.Printing;
using Windows.Storage.Streams;
using Windows.UI;
using GP.Windows;
using GP.Windows.UI;
using GP.Windows.UI.Controls;
using Hercules.Model.Layouting;
using Hercules.Model.Utils;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Printing;
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
            get { return zoomFactor; }
        }

        public ICanvasControl Canvas
        {
            get { return canvas; }
        }

        public Document Document
        {
            get { return document; }
        }

        public Win2DResourceManager Resources
        {
            get { return resources; }
        }

        public ICollection<Win2DRenderNode> RenderNodes
        {
            get { return renderNodes.Values; }
        }

        public ICollection<Win2DRenderNode> CustomNodes
        {
            get { return customNodes; }
        }

        public ICollection<Win2DRenderNode> AllNodes
        {
            get { return new Win2DRenderNode[] { previewNode }.Union(renderNodes.Values).Union(customNodes).ToList(); }
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
            document.NodeSelected += Document_NodeSelected;
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
            document.NodeSelected -= Document_NodeSelected;
            document.NodeRemoved -= Document_NodeRemoved;
            document.NodeAdded -= Document_NodeAdded;

            foreach (NodeBase node in document.Nodes)
            {
                TryRemove(node);
            }
        }

        private void InitializeCanvas()
        {
            canvas.CreateResources += Canvas_CreateResources;

            canvas.Draw += Canvas_Draw;
        }

        private void ReleaseCanvas()
        {
            canvas.CreateResources -= Canvas_CreateResources;

            canvas.Draw -= Canvas_Draw;
        }

        private void Canvas_CreateResources(object sender, EventArgs e)
        {
            resources.ClearResources();

            foreach (Win2DRenderNode renderNode in AllNodes)
            {
                renderNode.ClearResources();
            }

            Invalidate();
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

        public void InvalidateWithoutLayout()
        {
            canvas.Invalidate();
        }

        private void Canvas_Draw(object sender, CanvasDrawEventArgs args)
        {
            Render(args.DrawingSession);
        }

        public IPrintDocumentSource Print()
        {
            CanvasPrintDocument printDocument = new CanvasPrintDocument();

            Action<CanvasDrawingSession, PrintPageDescription> renderForPrint = (session, page) =>
            {
                session.Clear(Colors.White);

                float padding = 20;

                Vector2 size = page.PageSize.ToVector2();

                float ratio = sceneBounds.Width / sceneBounds.Height;

                float targetSizeX = Math.Min(size.X - (2 * padding), sceneBounds.Width);
                float targetSizeY = targetSizeX / ratio;

                if (targetSizeY > page.PageSize.Height)
                {
                    targetSizeY = Math.Min(size.Y - (2 * padding), sceneBounds.Height);
                    targetSizeX = targetSizeY * ratio;
                }

                float zoom = targetSizeX / sceneBounds.Width;

                session.Transform =
                    Matrix3x2.CreateTranslation(
                        -sceneBounds.Position.X,
                        -sceneBounds.Position.Y) *
                    Matrix3x2.CreateScale(zoom) *
                    Matrix3x2.CreateTranslation(
                         0.5f * (size.X - targetSizeX),
                         0.5f * (size.Y - targetSizeY));

                RenderIt(session, RenderFlags.Plain, Rect2.Infinite);
            };

            printDocument.Preview += (sender, args) =>
            {
                sender.SetPageCount(1);

                renderForPrint(args.DrawingSession, args.PrintTaskOptions.GetPageDescription(1));
            };

            printDocument.Print += (sender, args) =>
            {
                using (CanvasDrawingSession session = args.CreateDrawingSession())
                {
                    renderForPrint(session, args.PrintTaskOptions.GetPageDescription(1));
                }
            };

            return printDocument;
        }

        public async Task RenderScreenshotAsync(IRandomAccessStream stream, Color background, float dpi, float padding = 20)
        {
            Guard.NotNull(stream, nameof(stream));
            Guard.GreaterThan(padding, 0, nameof(padding));

            float w = sceneBounds.Size.X + (2 * padding);
            float h = sceneBounds.Size.Y + (2 * padding);

            dpi = dpi == 0 ? DisplayInformation.GetForCurrentView().LogicalDpi : dpi;

            using (CanvasRenderTarget target = new CanvasRenderTarget(canvas.Device, w, h, dpi))
            {
                using (CanvasDrawingSession session = target.CreateDrawingSession())
                {
                    session.Clear(background);

                    session.Transform =
                        Matrix3x2.CreateTranslation(
                            -sceneBounds.Position.X + padding,
                            -sceneBounds.Position.Y + padding);

                    RenderIt(session, RenderFlags.Plain, Rect2.Infinite);
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

                RenderIt(session, RenderFlags.Full, visibleRect);

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

            foreach (Win2DRenderNode renderNode in AllNodes)
            {
                renderNode.Measure(session);
            }

            if (layoutInvalidated)
            {
                layout.UpdateLayout(document, this);

                foreach (Win2DRenderNode renderNode in renderNodes.Values)
                {
                    if (renderNode.IsVisible)
                    {
                        renderNode.ComputeHull(session);
                    }
                }

                layoutInvalidated = false;
            }
        }

        private void UpdateArrangement(CanvasDrawingSession session, bool animate, out bool needsRedraw)
        {
            needsRedraw = false;

            DateTime utcNow = DateTime.UtcNow;

            foreach (Win2DRenderNode renderNode in AllNodes)
            {
                needsRedraw |= renderNode.AnimateRenderPosition(animate && !IsCustomNode(renderNode), utcNow, 600);
            }

            double minX = double.MaxValue;
            double minY = double.MaxValue;
            double maxX = double.MinValue;
            double maxY = double.MinValue;

            foreach (Win2DRenderNode renderNode in AllNodes)
            {
                renderNode.Arrange(session);

                if (renderNode.IsVisible && !IsCustomNode(renderNode))
                {
                    Rect2 bounds = renderNode.Bounds;

                    minX = Math.Min(minX, bounds.Left);
                    minY = Math.Min(minY, bounds.Top);
                    maxX = Math.Max(maxX, bounds.Right);
                    maxY = Math.Max(maxY, bounds.Bottom);
                }
            }

            foreach (Win2DRenderNode renderNode in AllNodes)
            {
                if (renderNode.IsVisible && !IsCustomNode(renderNode))
                {
                    renderNode.ComputePath(session);
                }
            }

            sceneBounds = new Rect2((float)minX, (float)minY, (float)(maxX - minX), (float)(maxY - minY));
        }

        private void RenderIt(CanvasDrawingSession session, RenderFlags renderFlags, Rect2 viewRect)
        {
            session.TextAntialiasing = CanvasTextAntialiasing.Grayscale;

            foreach (Win2DRenderNode renderNode in customNodes.Union(new Win2DRenderNode[] { previewNode }))
            {
                if (renderNode.IsVisible)
                {
                    renderNode.ComputePath(session);
                }
            }

            foreach (Win2DRenderNode renderNode in renderNodes.Values)
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

            bool renderCustoms = (renderFlags & RenderFlags.RenderCustoms) == RenderFlags.RenderCustoms;
            bool renderControls = (renderFlags & RenderFlags.RenderControls) == RenderFlags.RenderControls;

            foreach (Win2DRenderNode renderNode in AllNodes)
            {
                if (renderNode.IsVisible && CanRenderNode(renderNode, viewRect))
                {
                    bool isCustomNode = IsCustomNode(renderNode);

                    if (renderCustoms || !isCustomNode)
                    {
                        renderNode.Render(session, renderControls && !isCustomNode);
                    }
                }
            }
        }

        private static bool CanRenderPath(Win2DRenderNode renderNode, Rect2 viewRect)
        {
            return viewRect.IntersectsWith(renderNode.BoundsWithParent);
        }

        private static bool CanRenderNode(Win2DRenderNode renderNode, Rect2 viewRect)
        {
            return viewRect.IntersectsWith(renderNode.Bounds);
        }

        private bool IsCustomNode(Win2DRenderNode renderNode)
        {
            return customNodes.Contains(renderNode) || renderNode == previewNode;
        }

        public void AddCustomNode(Win2DRenderNode renderNode)
        {
            customNodes.Add(renderNode);
        }

        public void RemoveCustomNode(Win2DRenderNode renderNode)
        {
            customNodes.Remove(renderNode);
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

        private void Document_NodeSelected(object sender, NodeEventArgs e)
        {
            InvalidateWithoutLayout();
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

        public ThemeColor FindColor(NodeBase node)
        {
            return resources.FindColor(node);
        }

        protected abstract Win2DRenderNode CreatePreviewNode();

        protected abstract Win2DRenderNode CreateRenderNode(NodeBase node);
    }
}
