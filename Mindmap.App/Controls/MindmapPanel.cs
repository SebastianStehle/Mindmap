// ==========================================================================
// Mindmap.cs
// Mindmap Application
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using MindmapApp.Model;
using MindmapApp.Model.Layouting;
using GreenParrot.Windows;
using GreenParrot.Windows.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;

namespace MindmapApp.Controls
{
    public sealed class MindmapPanel : Panel, IRenderer
    {
        private const double AnimationSpeed = 600;
        private readonly Dictionary<NodeBase, NodeContainer> nodeControls = new Dictionary<NodeBase, NodeContainer>();
        private readonly NodeContainer previewContainer;
        private bool requiresLayout = true;
        private int canArrangeIfZero;

        public static readonly DependencyProperty ThemeProperty =
            DependencyProperty.Register("Theme", typeof(ThemeBase), typeof(MindmapPanel), new PropertyMetadata(null, new PropertyChangedCallback(OnThemeChanged)));
        public ThemeBase Theme
        {
            get { return (ThemeBase)GetValue(ThemeProperty); }
            set { SetValue(ThemeProperty, value); }
        }

        private static void OnThemeChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            var owner = o as MindmapPanel;
            if (owner != null)
            {
                owner.OnThemeChanged(e);
            }
        }

        private void OnThemeChanged(DependencyPropertyChangedEventArgs e)
        {
            MakeTransactionalLayoutChange(() =>
            {
                ThemeBase Theme = e.NewValue as ThemeBase;

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
                    x.PathHolder = Theme.CreatePath();

                    VisualTreeExtensions.TryAdd(this, x.PathHolder.Path);
                };

                foreach (NodeContainer renderContainer in nodeControls.Values)
                {
                    cleanUpContainer(renderContainer);
                }

                cleanUpContainer(previewContainer);

                if (Theme != null)
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
            });
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
            MakeTransactionalLayoutChange(() =>
            {
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
            });
        }

        public MindmapPanel()
        {
            previewContainer = new NodeContainer(new NodeControl());

            VisualTreeExtensions.TryAdd(this, previewContainer.NodeControl);
        }

        public void ShowPreviewElement(Point? position, NodeBase parent, AnchorPoint anchor)
        {
            MakeTransactionalLayoutChange(() =>
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
            });
        }

        private void HandleDocumentAdded(Document newDocument)
        {
            MakeTransactionalLayoutChange(() =>
            {
                newDocument.StateChanged += UndoRedoManager_StateChanged;
                newDocument.NodeRemoved += oldDocument_NodeRemoved;
                newDocument.NodeAdded += oldDocument_NodeAdded;

                foreach (NodeBase node in newDocument.Nodes)
                {
                    HandleNodeAdded(node);
                }
            });
        }

        private void HandleDocumentRemoved(Document oldDocument)
        {
            MakeTransactionalLayoutChange(() =>
            {
                oldDocument.StateChanged -= UndoRedoManager_StateChanged;
                oldDocument.NodeRemoved -= oldDocument_NodeRemoved;
                oldDocument.NodeAdded -= oldDocument_NodeAdded;

                foreach (NodeBase node in nodeControls.Keys.ToList())
                {
                    HandleNodeRemoved(node);
                }
            });
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
            MakeTransactionalLayoutChange(() =>
            {
                NodeContainer nodeContainer = node.RenderData as NodeContainer;

                if (nodeContainer != null)
                {
                    NodeControl nodeControl = nodeContainer.NodeControl;
                    nodeControl.Detach();
                    nodeControls.Remove(node);

                    node.RenderData = null;

                    VisualTreeExtensions.TryRemove(this, nodeControl);

                    if (nodeContainer.PathHolder != null)
                    {
                        VisualTreeExtensions.TryRemove(this, nodeContainer.PathHolder.Path);
                    }
                }
            });
        }

        private NodeContainer TryGetContainer(NodeBase node)
        {
            if (node != null)
            {
                return nodeControls.GetOrCreateDefault(node, () =>
                {
                    NodeContainer nodeContainer = null;

                    MakeTransactionalLayoutChange(() =>
                    {
                        NodeControl nodeControl = new NodeControl();
                        nodeControl.Attach(node);
                        nodeContainer = new NodeContainer(nodeControl) { Parent = TryGetContainer(node.Parent) };

                        Node normalNode = node as Node;

                        if (normalNode != null && Theme != null)
                        {
                            nodeContainer.PathHolder = Theme.CreatePath();

                            VisualTreeExtensions.TryAdd(this, nodeContainer.PathHolder.Path, 0);
                        }

                        VisualTreeExtensions.TryAdd(this, nodeControl, 1000000);

                        node.RenderData = nodeContainer;
                    });

                    return nodeContainer;
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
            Document document = Document;

            if (document != null)
            {
                ThemeBase theme = Theme;

                if (theme != null && Layout != null)
                {
                    if (requiresLayout && canArrangeIfZero == 0)
                    {
                        UpdateMindmap(document);

                        requiresLayout = false;
                    }

                    UpdatePositions();

                    RenderPaths(Theme, finalSize);
                }
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

        private void UpdateStyles(ThemeBase theme)
        {
            theme.UpdateStyle(previewContainer, true);

            foreach (NodeContainer renderContainer in nodeControls.Values)
            {
                if (renderContainer.NodeControl != null && renderContainer.NodeControl.AssociatedNode != null)
                {
                    theme.UpdateStyle(renderContainer, false);
                }
            }
        }

        private void UpdateMindmap(Document document)
        {
            Layout.UpdateLayout(document, this);
        }

        private void RenderPaths(ThemeBase theme, Size finalSize)
        {
            foreach (Path path in Children.OfType<Path>())
            {
                path.Arrange(new Rect(new Point(0, 0), finalSize));
            }

            foreach (NodeContainer renderContainer in nodeControls.Values)
            {
                if (renderContainer.PathHolder != null)
                {
                    theme.RenderPath(renderContainer.PathHolder, renderContainer);
                }
            }

            if (previewContainer.PathHolder != null && previewContainer.Parent != null)
            {
                theme.RenderPath(previewContainer.PathHolder, previewContainer);
            }
        }

        protected override Size MeasureOverride(Size constraint)
        {
            ThemeBase theme = Theme;

            if (theme != null)
            {
                UpdateStyles(theme);

                requiresLayout = true;

                Size availableSize = new Size(double.PositiveInfinity, double.PositiveInfinity);
                
                foreach (UIElement child in Children)
                {
                    if (child != null)
                    {
                        child.Measure(availableSize);

                        if (child is NodeControl && child.Visibility == Visibility.Visible && child != previewContainer.NodeControl && child.DesiredSize == new Size(0, 0))
                        {
                            requiresLayout = false;
                        }
                    }
                }
            }

            return default(Size);
        }

        private void MakeTransactionalLayoutChange(Action action)
        {
            canArrangeIfZero++;
            try
            {
                action();
            }
            finally
            {
                canArrangeIfZero--;
            }
        }

        private void CompositionTarget_Rendering(object sender, object e)
        {
            InvalidateArrange();
        }

        public Rect GetBounds(NodeBase node)
        {
            return ((NodeContainer)node.RenderData).Bounds;
        }

        public NodeControl GetControl(NodeBase node)
        {
            return ((NodeContainer)node.RenderData).NodeControl;
        }

        public IRenderNode FindRenderNode(NodeBase node)
        {
            return (NodeContainer)node.RenderData;
        }
    }
}
