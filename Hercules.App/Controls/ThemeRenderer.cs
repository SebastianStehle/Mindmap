using System.Collections.Generic;
using System.Linq;
using Hercules.Model;
using GP.Windows;
using Hercules.Model.Layouting;
using Microsoft.Graphics.Canvas;
using GP.Windows.UI;
using Microsoft.Graphics.Canvas.Brushes;
using Windows.UI;
using Hercules.Model.Utils;
using System.Numerics;
using Microsoft.Graphics.Canvas.UI.Xaml;
using System;
using Windows.UI.Input;

namespace Hercules.App.Controls
{
    public abstract class ThemeRenderer : IRenderer
    {
        private readonly Dictionary<NodeBase, ThemeRenderNode> renderNodes = new Dictionary<NodeBase, ThemeRenderNode>();
        private readonly List<ThemeColor> colors = new List<ThemeColor>();
        private CanvasControl currentCanvas;
        private Document currentDocument;
        private Matrix3x2 transform = Matrix3x2.Identity;
        private Matrix3x2 scale = Matrix3x2.Identity;
        private Matrix3x2 inverseTransform = Matrix3x2.Identity;
        private Matrix3x2 inverseScale = Matrix3x2.Identity;
        private Rect2 visibleRect = new Rect2(0, 0, float.PositiveInfinity, float.PositiveInfinity);
        private float zoomFactor;
        private ICanvasBrush pathBrush;
        private ILayout layout;

        public float ZoomFactor
        {
            get
            {
                return zoomFactor;
            }
        }

        public IEnumerable<ThemeRenderNode> RenderNodes
        {
            get
            {
                return renderNodes.Values;
            }
        }

        public IReadOnlyList<ThemeColor> Colors
        {
            get
            {
                return colors;
            }
        }

        public void Initialize(Document document, ILayout layout, CanvasControl canvas)
        {
            this.layout = layout;

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

        public void Transform(Vector2 translate, float zoom, Rect2 visibleRect)
        {
            this.visibleRect = visibleRect;
            
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
            if (currentDocument != null && layout != null)
            {
                session.Units = CanvasUnits.Dips;
                session.Transform = transform;

                layout.UpdateVisibility(currentDocument, this);
                
                foreach (ThemeRenderNode nodeContainer in renderNodes.Values)
                {
                    nodeContainer.Measure(session);
                }

                layout.UpdateLayout(currentDocument, this);

                foreach (ThemeRenderNode nodeContainer in renderNodes.Values)
                {
                    nodeContainer.Arrange(session);
                }

                int nodes = 0;
                int paths = 0;

                foreach (ThemeRenderNode nodeContainer in renderNodes.Values)
                {
                    if (nodeContainer.IsVisible && CanRenderPath(nodeContainer))
                    {
                        paths++;

                        nodeContainer.RenderPath(session);
                    }
                }

                foreach (ThemeRenderNode nodeContainer in renderNodes.Values)
                {
                    if (nodeContainer.IsVisible && CanRenderNode(nodeContainer))
                    {
                        nodes++;

                        nodeContainer.Render(session);
                    }
                }
            }
        }

        private bool CanRenderPath(ThemeRenderNode nodeContainer)
        {
            return visibleRect.IntersectsWith(nodeContainer.TotalBounds);
        }

        private bool CanRenderNode(ThemeRenderNode nodeContainer)
        {
            return visibleRect.IntersectsWith(nodeContainer.Bounds);
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

        public bool HandleClick(Vector2 position, out ThemeRenderNode handledNode)
        {
            handledNode = null;

            foreach (ThemeRenderNode renderNode in renderNodes.Values)
            {
                if (renderNode.HandleClick(position))
                {
                    handledNode = renderNode;

                    if (currentCanvas != null)
                    {
                        currentCanvas.Invalidate();
                    }
                    
                    return true;
                }
            }

            return false;
        }

        public ThemeColor FindColor(NodeBase node)
        {
            return Colors[node.Color];
        }

        private void Document_StateChanged(object sender, System.EventArgs e)
        {
            if (currentCanvas != null)
            {
                currentCanvas.Invalidate();
            }
        }

        private void Document_NodeAdded(object sender, NodeEventArgs e)
        {
            TryAdd(e.Node);
        }

        private void Document_NodeRemoved(object sender, NodeEventArgs e)
        {
            TryRemove(e.Node);
        }

        private bool TryRemove(NodeBase node)
        {
            if (node != null)
            {
                return renderNodes.Remove(node);
            }

            return false;
        }

        private ThemeRenderNode TryAdd(NodeBase node)
        {
            if (node != null)
            {
                return renderNodes.GetOrCreateDefault(node, () =>
                {
                    ThemeRenderNode renderNode = CreateRenderNode(node);

                    renderNode.Parent = TryAdd(node.Parent);

                    return renderNode;
                });
            }

            return null;
        }

        public ICanvasBrush PathBrush(CanvasDrawingSession session)
        {
            Guard.NotNull(session, nameof(session));

            return pathBrush ?? (pathBrush = new CanvasSolidColorBrush(session.Device, Color.FromArgb(255, 30, 30, 30)));
        }

        public IRenderNode FindRenderNode(NodeBase node)
        {
            return TryAdd(node);
        }

        protected abstract ThemeRenderNode CreateRenderNode(NodeBase node);
    }
}
