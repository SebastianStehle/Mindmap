// ==========================================================================
// Mindmap.cs
// RavenMind Application
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using RavenMind.Model;
using RavenMind.Model.Layouting;
using SE.Metro;
using SE.Metro.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;

namespace RavenMind.Controls
{
    public sealed class MindmapPanel : Panel, IRenderer
    {
        private const double AnimationSpeed = 600;
        private readonly Dictionary<NodeBase, NodeContainer> nodeControls = new Dictionary<NodeBase, NodeContainer>();
        private readonly NodeContainer previewContainer;
        private bool requiresLayout = true;

        public static readonly DependencyProperty RendererProperty =
            DependencyProperty.Register("Renderer", typeof(RendererBase), typeof(MindmapPanel), new PropertyMetadata(null, new PropertyChangedCallback(OnRendererChanged)));
        public RendererBase Renderer
        {
            get { return (RendererBase)GetValue(RendererProperty); }
            set { SetValue(RendererProperty, value); }
        }

        private static void OnRendererChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            var owner = o as MindmapPanel;
            if (owner != null)
            {
                owner.OnRendererChanged(e);
            }
        }

        private void OnRendererChanged(DependencyPropertyChangedEventArgs e)
        {
            requiresLayout = false;

            RendererBase renderer = e.NewValue as RendererBase;

            Action<NodeContainer> cleanUpContainer = x =>
            {
                if (x.PathHolder != null)
                {
                    VisualTreeExtensions.TryRemove(this, x.PathHolder.Path);

                    x.PathHolder = null;
                }
            };

            Action<NodeContainer> setupContainer = x =>
            {
                x.PathHolder = renderer.CreatePath();

                VisualTreeExtensions.TryAdd(this, x.PathHolder.Path);
            };

            foreach (NodeContainer renderContainer in nodeControls.Values)
            {
                cleanUpContainer(renderContainer);
            }

            cleanUpContainer(previewContainer);

            if (renderer != null)
            {
                foreach (NodeContainer renderContainer in nodeControls.Values)
                {
                    if (renderContainer.Parent != null)
                    {
                        setupContainer(renderContainer);
                    }
                }

                setupContainer(previewContainer);
            }

            requiresLayout = true;
        }

        public static readonly DependencyProperty IsAnimatingProperty =
            DependencyProperty.Register("IsAnimating", typeof(bool), typeof(MindmapPanel), new PropertyMetadata(true));
        public bool IsAnimating
        {
            get { return (bool)GetValue(IsAnimatingProperty); }
            set { SetValue(IsAnimatingProperty, value); }
        }

        public static readonly DependencyProperty LayoutProperty =
            DependencyProperty.Register("Layout", typeof(ILayout), typeof(MindmapPanel), new PropertyMetadata(null));
        public ILayout Layout
        {
            get { return (ILayout)GetValue(LayoutProperty); }
            set { SetValue(LayoutProperty, value); }
        }

        public static readonly DependencyProperty DocumentProperty =
            DependencyProperty.Register("Document", typeof(Document), typeof(MindmapPanel), new PropertyMetadata(null, new PropertyChangedCallback(OnDocumentChanged)));
        public Document Document
        {
            get { return (Document)GetValue(DocumentProperty); }
            set { SetValue(DocumentProperty, value); }
        }

        private static void OnDocumentChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            var owner = o as MindmapPanel;
            if (owner != null)
            {
                owner.OnDocumentChanged(e);
            }
        }

        private void OnDocumentChanged(DependencyPropertyChangedEventArgs e)
        {
            requiresLayout = false;

            Document oldDocument = e.OldValue as Document;

            if (oldDocument != null)
            {
                HandleDocumentRemoved(oldDocument);
            }

            Document newDocument = e.NewValue as Document;

            if (newDocument != null)
            {
                HandleDocumentAdded(newDocument);
            }

            requiresLayout = true;
        }

        public MindmapPanel()
        {
            previewContainer = new NodeContainer(new NodeControl());

            VisualTreeExtensions.TryAdd(this, previewContainer.NodeControl);
        }

        public void ShowPreviewElement(Point? position, NodeBase parent, AnchorPoint anchor)
        {
            if (position.HasValue)
            {
                NodeContainer parentContainer;

                if (nodeControls.TryGetValue(parent, out parentContainer))
                {
                    previewContainer.MoveTo(position.Value, anchor);
                    previewContainer.Parent = parentContainer;

                    if (previewContainer.PathHolder != null)
                    {
                        previewContainer.PathHolder.Path.Visibility = Visibility.Visible;
                    }

                    previewContainer.NodeControl.Visibility = Visibility.Visible;
                }
            }
            else
            {
                previewContainer.Parent = null;

                if (previewContainer.PathHolder != null)
                {
                    previewContainer.PathHolder.Path.Visibility = Visibility.Collapsed;
                }

                previewContainer.NodeControl.Visibility = Visibility.Collapsed;
            }
        }

        private void HandleDocumentAdded(Document newDocument)
        {
            newDocument.StateChanged += UndoRedoManager_StateChanged;
            newDocument.NodeRemoved += oldDocument_NodeRemoved;
            newDocument.NodeAdded += oldDocument_NodeAdded;

            foreach (NodeBase node in newDocument.Nodes)
            {
                HandleNodeAdded(node);
            }
        }

        private void HandleDocumentRemoved(Document oldDocument)
        {
            oldDocument.StateChanged -= UndoRedoManager_StateChanged;
            oldDocument.NodeRemoved -= oldDocument_NodeRemoved;
            oldDocument.NodeAdded -= oldDocument_NodeAdded;

            foreach (NodeBase node in nodeControls.Keys.ToList())
            {
                HandleNodeRemoved(node);
            }
        }

        private void oldDocument_NodeRemoved(object sender, NodeEventArgs e)
        {
            HandleNodeRemoved(e.Node);
        }

        private void oldDocument_NodeAdded(object sender, NodeEventArgs e)
        {
            HandleNodeAdded(e.Node);
        }

        private void HandleNodeAdded(NodeBase node)
        {
            TryGetContainer(node);
        }

        private void HandleNodeRemoved(NodeBase node)
        {
            TryRemove(node);
        }

        private void TryRemove(NodeBase node)
        {
            NodeContainer renderContainer = nodeControls[node];

            NodeControl nodeControl = renderContainer.NodeControl;
            nodeControl.Detach();
            nodeControls.Remove(node);

            VisualTreeExtensions.TryRemove(this, nodeControl);

            if (renderContainer.PathHolder != null)
            {
                VisualTreeExtensions.TryRemove(this, renderContainer.PathHolder.Path);
            }
        }

        private NodeContainer TryGetContainer(NodeBase node)
        {
            if (node != null)
            {
                return nodeControls.GetOrCreateDefault(node, () =>
                {
                    NodeControl nodeControl = new NodeControl();
                    nodeControl.Attach(node);

                    NodeContainer renderContainer = new NodeContainer(nodeControl) { Parent = TryGetContainer(node.Parent) };

                    Node normalNode = node as Node;

                    if (normalNode != null && Renderer != null)
                    {
                        renderContainer.PathHolder = Renderer.CreatePath();

                        VisualTreeExtensions.TryAdd(this, renderContainer.PathHolder.Path);
                    }

                    VisualTreeExtensions.TryAdd(this, nodeControl);

                    return renderContainer;
                });
            }

            return null;
        }

        public void InvalidateLayout()
        {
            requiresLayout = true;

            InvalidateMeasure();
            InvalidateArrange();
        }

        private void UndoRedoManager_StateChanged(object sender, EventArgs e)
        {
            InvalidateLayout();
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            RendererBase renderer = Renderer;

            if (renderer != null && Layout != null)
            {
                UpdateStyles(renderer);
                UpdateLayout(finalSize);
                UpdatePositions();

                RenderPaths(renderer, finalSize);
            }

            return base.ArrangeOverride(finalSize);
        }

        private void UpdatePositions()
        {
            bool isAnimating = false;

            DateTime now = DateTime.Now;

            foreach (NodeContainer renderContainer in nodeControls.Values)
            {
                if (renderContainer.UpdateRenderPosition(IsAnimating, now, AnimationSpeed))
                {
                    isAnimating = true;
                }
            }

            previewContainer.UpdateRenderPosition(false, now, AnimationSpeed);

            CompositionTarget.Rendering -= CompositionTarget_Rendering;

            if (isAnimating)
            {
                CompositionTarget.Rendering += CompositionTarget_Rendering;
            }
        }

        private void UpdateStyles(RendererBase renderer)
        {
            previewContainer.NodeControl.Style = renderer.CalculateStyle(previewContainer, true);

            foreach (NodeContainer renderContainer in nodeControls.Values)
            {
                renderContainer.NodeControl.Style = renderer.CalculateStyle(renderContainer, false);
            }
        }

        private void UpdateLayout(Size finalSize)
        {
            if (requiresLayout)
            {
                Layout.UpdateLayout(Document, this, finalSize);

                requiresLayout = false;
            }
        }

        private void RenderPaths(RendererBase renderer, Size finalSize)
        {
            foreach (Path path in Children.OfType<Path>())
            {
                path.Arrange(new Rect(new Point(0, 0), finalSize));
            }

            foreach (NodeContainer renderContainer in nodeControls.Values)
            {
                if (renderContainer.PathHolder != null)
                {
                    renderer.RenderPath(renderContainer.PathHolder, renderContainer);
                }
            }

            if (previewContainer.PathHolder != null && previewContainer.Parent != null)
            {
                renderer.RenderPath(previewContainer.PathHolder, previewContainer);
            }
        }

        protected override Size MeasureOverride(Size constraint)
        {
            requiresLayout = true;

            Size availableSize = new Size(double.PositiveInfinity, double.PositiveInfinity);

            foreach (UIElement child in Children)
            {
                if (child != null)
                {
                    child.Measure(availableSize);
                }
            }

            return default(Size);
        }

        private void CompositionTarget_Rendering(object sender, object e)
        {
            InvalidateArrange();
        }

        public Rect GetBounds(NodeBase node)
        {
            return nodeControls[node].Bounds;
        }

        public NodeControl GetControl(NodeBase node)
        {
            return nodeControls[node].NodeControl;
        }

        public IRenderNode FindRenderNode(NodeBase node)
        {
            return nodeControls[node];
        }
    }
}
