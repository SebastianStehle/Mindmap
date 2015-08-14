// ==========================================================================
// ThemeRenderer.cs
// Metro Library SE
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using Windows.Storage;
using GP.Windows;
using GP.Windows.UI;
using Hercules.Model.Layouting;
using Hercules.Model.Utils;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Brushes;
using Microsoft.Graphics.Canvas.Text;
using Microsoft.Graphics.Canvas.UI.Xaml;
using Windows.UI;
using Windows.UI.Core;

// ReSharper disable UnusedParameter.Local
// ReSharper disable DoNotCallOverridableMethodsInConstructor

namespace Hercules.Model.Rendering.Win2D
{
    public abstract class Win2DRenderer : IRenderer
    {
        private readonly Dictionary<string, ImageContainer> images = new Dictionary<string, ImageContainer>();
        private readonly Dictionary<NodeBase, Win2DRenderNode> renderNodes = new Dictionary<NodeBase, Win2DRenderNode>();
        private readonly List<ThemeColor> colors = new List<ThemeColor>();
        private readonly List<Win2DRenderNode> customNodes = new List<Win2DRenderNode>();
        private readonly Win2DRenderNode previewNode;
        private CanvasControl currentCanvas;
        private Document currentDocument;
        private Matrix3x2 transform = Matrix3x2.Identity;
        private Matrix3x2 scale = Matrix3x2.Identity;
        private Matrix3x2 inverseTransform = Matrix3x2.Identity;
        private Matrix3x2 inverseScale = Matrix3x2.Identity;
        private Rect2 visibleRect = new Rect2(0, 0, float.PositiveInfinity, float.PositiveInfinity);
        private float zoomFactor;
        private ICanvasBrush pathBrush;
        private ILayout currentLayout;

        private sealed class ImageContainer
        {
            public CanvasBitmap Bitmap;

            public ImageContainer(string image, CanvasDevice device, Win2DRenderer renderer)
            {
                LoadFile(image, device).ContinueWith(bitmap =>
                {
                    renderer.Invalidate();

                    Bitmap = bitmap.Result;
                });
            }

            private async Task<CanvasBitmap> LoadFile(string image, CanvasDevice device)
            {
                string uri = string.Format(CultureInfo.InvariantCulture, "ms-appx:///{0}", image);

                StorageFile file = await StorageFile.GetFileFromApplicationUriAsync(new Uri(uri));

                using (var stream = await file.OpenReadAsync())
                {
                    return await CanvasBitmap.LoadAsync(device, stream).AsTask();
                }
            }
        }

        public float ZoomFactor
        {
            get
            {
                return zoomFactor;
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

        public IReadOnlyList<ThemeColor> Colors
        {
            get
            {
                return colors;
            }
        }

        protected Win2DRenderer()
        {
            previewNode = CreatePreviewNode();
        }

        public void Initialize(Document document, ILayout layout, CanvasControl canvas)
        {
            currentLayout = layout;

            InitializeCanvas(canvas);
            InitializeDocument(document);
        }

        private void InitializeCanvas(CanvasControl canvas)
        {
            if (currentCanvas != canvas)
            {
                if (currentCanvas != null)
                {
                    currentCanvas.Draw -= Canvas_Draw;
                }

                currentCanvas = canvas;

                if (currentCanvas != null)
                {
                    currentCanvas.Draw += Canvas_Draw;
                }
            }
        }

        private void InitializeDocument(Document document)
        {
            if (currentDocument != document)
            {
                if (currentDocument != null)
                {
                    currentDocument.StateChanged -= Document_StateChanged;
                    currentDocument.NodeRemoved -= Document_NodeRemoved;
                    currentDocument.NodeAdded -= Document_NodeAdded;

                    foreach (NodeBase node in renderNodes.Keys.ToList())
                    {
                        TryRemove(node);
                    }
                }

                currentDocument = document;

                if (currentDocument != null)
                {
                    currentDocument.StateChanged += Document_StateChanged;
                    currentDocument.NodeRemoved += Document_NodeRemoved;
                    currentDocument.NodeAdded += Document_NodeAdded;

                    foreach (NodeBase node in currentDocument.Nodes)
                    {
                        TryAdd(node);
                    }
                }
            }
        }
        
        public void Invalidate()
        {
            if (currentCanvas != null)
            {
                if (currentCanvas.Dispatcher.HasThreadAccess)
                {
                    currentCanvas.Invalidate();
                }
                else
                {
                    currentCanvas.Dispatcher.RunAsync(CoreDispatcherPriority.High, () =>
                    {
                        currentCanvas.Invalidate();
                    }).AsTask();
                }
            }
        }

        public void Transform(Vector2 translate, float zoom, Rect2 rect)
        {
            this.visibleRect = rect;
            
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

        protected void AddColors(params int[] newColors)
        {
            foreach (int color in newColors)
            {
                colors.Add(new ThemeColor(
                    ColorsHelper.ConvertToColor(color, 0, 0, 0),
                    ColorsHelper.ConvertToColor(color, 0, 0.2, -0.3),
                    ColorsHelper.ConvertToColor(color, 0, -0.2, 0.2)));
            }
        }

        private void Canvas_Draw(CanvasControl sender, CanvasDrawEventArgs args)
        {
            CanvasDrawingSession session = args.DrawingSession;

            if (session != null)
            {
                Render(session);
            }
        }

        private void Render(CanvasDrawingSession session)
        {
            if (currentDocument != null && currentLayout != null)
            {
                session.TextAntialiasing = CanvasTextAntialiasing.Grayscale;

                session.Transform = transform;

                currentLayout.UpdateVisibility(currentDocument, this);

                List<Win2DRenderNode> allNodes = renderNodes.Values.Union(customNodes).Union(new Win2DRenderNode[] { previewNode }).ToList();
                
                foreach (Win2DRenderNode node in allNodes)
                {
                    node.Measure(session);
                }

                currentLayout.UpdateLayout(currentDocument, this);

                foreach (Win2DRenderNode node in allNodes)
                {
                    node.Arrange(session);
                }

                int nodes = 0;
                int paths = 0;

                foreach (Win2DRenderNode node in allNodes)
                {
                    if (node.IsVisible && CanRenderPath(node))
                    {
                        paths++;

                        node.RenderPath(session);
                    }
                }

                foreach (Win2DRenderNode node in allNodes)
                {
                    if (node.IsVisible && CanRenderNode(node))
                    {
                        nodes++;

                        node.Render(session);
                    }
                }

                Debug.WriteLine("Nodes: {0}, Paths: {1}", nodes, paths);
            }
        }

        public void HidePreviewElement()
        {
            previewNode.Hide();
        }

        public void ShowPreviewElement(Vector2 position, NodeBase parent, AnchorPoint anchor)
        {
            Win2DRenderNode parentNode = TryAdd(parent);

            previewNode.MoveTo(position, anchor);
            previewNode.Parent = parentNode;
            previewNode.Show();
        }

        private bool CanRenderPath(Win2DRenderNode node)
        {
            return visibleRect.IntersectsWith(node.BoundsWithParent);
        }

        private bool CanRenderNode(Win2DRenderNode node)
        {
            return visibleRect.IntersectsWith(node.Bounds);
        }

        public void AddCustomNode(Win2DRenderNode node)
        {
            customNodes.Add(node);
        }

        public void RemoveCustomNode(Win2DRenderNode node)
        {
            customNodes.Remove(node);
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

        public ThemeColor FindColor(NodeBase node)
        {
            return Colors[node.Color];
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

        public ICanvasBrush PathBrush(CanvasDrawingSession session, bool copy = false)
        {
            Guard.NotNull(session, nameof(session));

            Func<ICanvasBrush> createBrush = () => new CanvasSolidColorBrush(session.Device, Color.FromArgb(255, 30, 30, 30));

            if (copy)
            {
                return createBrush();
            }
            else
            {
                return pathBrush ?? (pathBrush = createBrush());
            }
        }

        public ICanvasImage Image(CanvasDrawingSession session, string image)
        {
            return images.GetOrCreateDefault(image, () => new ImageContainer(image, session.Device, this)).Bitmap;
        }

        public IRenderNode FindRenderNode(NodeBase node)
        {
            return TryAdd(node);
        }

        protected abstract Win2DRenderNode CreatePreviewNode();

        protected abstract Win2DRenderNode CreateRenderNode(NodeBase node);
    }
}
