using System;
using System.Collections.Generic;
using System.Linq;
using Hercules.Model;
using GP.Windows;
using Microsoft.Graphics.Canvas.UI.Xaml;
using Hercules.Model.Layouting;
using Microsoft.Graphics.Canvas;
using GP.Windows.UI;
using Microsoft.Graphics.Canvas.Brushes;
using Windows.UI;
using Hercules.Model.Utils;
using System.Diagnostics;

namespace Hercules.App.Controls
{
    public abstract class ThemeRenderer : IRenderer
    {
        private readonly Dictionary<NodeBase, ThemeRenderNode> renderNodes = new Dictionary<NodeBase, ThemeRenderNode>();
        private readonly List<ThemeColor> colors = new List<ThemeColor>();
        private Document currentDocument;
        private ICanvasBrush pathBrush;
        private ILayout layout;

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

        public void Initialize(Document document, ILayout layout)
        {
            this.layout = layout;
            
            InitializeDocument(document);
        }

        protected void AddColors(params int[] newColors)
        {
            foreach (int color in newColors)
            {
                colors.Add(new ThemeColor(
                    ColorsHelper.ConvertToColor((int)color, 0, 0, 0),
                    ColorsHelper.ConvertToColor((int)color, 0, 0.2, -0.3),
                    ColorsHelper.ConvertToColor((int)color, 0, -0.2, 0.2)));
            }
        }

        public void Render(CanvasDrawingSession session, Rect2 bounds)
        {
            if (currentDocument != null && layout != null)
            {
                layout.UpdateVisibility(currentDocument, this);
                
                foreach (ThemeRenderNode nodeContainer in renderNodes.Values)
                {
                    nodeContainer.Measure(session);
                }

                layout.UpdateLayout(currentDocument, this);

                int nodes = 0;
                int paths = 0;

                foreach (ThemeRenderNode nodeContainer in renderNodes.Values)
                {
                    if (nodeContainer.IsVisible && CanRenderPath(ref bounds, nodeContainer))
                    {
                        paths++;

                        nodeContainer.RenderPath(session);
                    }
                }

                foreach (ThemeRenderNode nodeContainer in renderNodes.Values)
                {
                    if (nodeContainer.IsVisible && CanRenderNode(ref bounds, nodeContainer))
                    {
                        nodes++;

                        nodeContainer.Render(session);
                    }
                }

                //Debug.WriteLine("Rendering: {0} Nodes, {1} Paths", nodes,paths);
            }
        }

        private static bool CanRenderPath(ref Rect2 bounds, ThemeRenderNode nodeContainer)
        {
            return bounds.IntersectsWith(nodeContainer.Bounds) || (nodeContainer.Parent != null && bounds.IntersectsWith(nodeContainer.Parent.Bounds));
        }

        private static bool CanRenderNode(ref Rect2 bounds, ThemeRenderNode nodeContainer)
        {
            return bounds.IntersectsWith(nodeContainer.Bounds);
        }

        private void InitializeDocument(Document document)
        {
            if (currentDocument != null)
            {
                currentDocument.NodeRemoved -= Document_NodeAdded;
                currentDocument.NodeAdded -= Document_NodeAdded;

                foreach (NodeBase node in renderNodes.Keys.ToList())
                {
                    TryRemove(node);
                }
            }

            currentDocument = document;

            if (currentDocument != null)
            {
                currentDocument.NodeRemoved += Document_NodeAdded;
                currentDocument.NodeAdded += Document_NodeAdded;

                foreach (NodeBase node in currentDocument.Nodes)
                {
                    TryAdd(node);
                }
            }
        }

        public void Clear()
        {
            foreach (NodeBase node in renderNodes.Keys.ToList())
            {
                TryRemove(node);
            }
        }

        public ThemeColor FindColor(NodeBase node)
        {
            return Colors[node.Color];
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

            return pathBrush ?? (pathBrush = new CanvasSolidColorBrush(session.Device, Color.FromArgb(200, 0, 0, 0)));
        }

        public IRenderNode FindRenderNode(NodeBase node)
        {
            return TryAdd(node);
        }

        protected abstract ThemeRenderNode CreateRenderNode(NodeBase node);
    }
}
