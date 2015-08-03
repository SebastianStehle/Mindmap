// ==========================================================================
// Mindmap.cs
// Mindmap Application
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using Hercules.Model;
using Hercules.Model.Layouting;
using GP.Windows;
using GP.Windows.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Shapes;
using System.Diagnostics;

namespace Hercules.App.Controls
{
    public sealed class MindmapPanel : Panel, IRenderer
    {
        private const double AnimationSpeed = 600;
        private readonly Dictionary<NodeBase, NodeContainer> nodeControls = new Dictionary<NodeBase, NodeContainer>();
        private readonly NodeContainer previewContainer;
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
                        this.TryRemove(x.PathHolder.Path);

                        x.PathHolder = null;
                    }
                };

                Action<NodeContainer> setupContainer = x =>
                {
                    x.PathHolder = Theme.CreatePath();

                    this.TryAdd(x.PathHolder.Path);
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

            InvalidateLayout();
        }

        public static readonly DependencyProperty IsAnimatingProperty =
            DependencyProperty.Register("IsAnimating", typeof(bool), typeof(MindmapPanel), new PropertyMetadata(true));
        public bool IsAnimating
        {
            get { return (bool)GetValue(IsAnimatingProperty); }
            set { SetValue(IsAnimatingProperty, value); }
        }

        public static readonly DependencyProperty LayoutProperty =
            DependencyProperty.Register("Layout", typeof(ILayout), typeof(MindmapPanel), new PropertyMetadata(null, new PropertyChangedCallback(OnLayoutChanged)));
        public ILayout Layout
        {
            get { return (ILayout)GetValue(LayoutProperty); }
            set { SetValue(LayoutProperty, value); }
        }

        private static void OnLayoutChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            var owner = o as MindmapPanel;
            if (owner != null)
            {
                owner.OnLayoutChanged(e);
            }
        }

        private void OnLayoutChanged(DependencyPropertyChangedEventArgs e)
        {
            InvalidateLayout();
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

            this.TryAdd(previewContainer.NodeControl);
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

                InvalidateArrange();
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

                    this.TryRemove(nodeControl);

                    if (nodeContainer.PathHolder != null)
                    {
                        this.TryRemove(nodeContainer.PathHolder.Path);
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

                            this.TryAdd(nodeContainer.PathHolder.Path, 0);
                        }

                        this.TryAdd(nodeControl, 1000000);

                        node.RenderData = nodeContainer;
                    });

                    return nodeContainer;
                });
            }

            return null;
        }

        private void InvalidateLayout()
        {
            if (canArrangeIfZero == 0)
            {
                InvalidateMeasure();
                InvalidateArrange();
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

        private void UndoRedoManager_StateChanged(object sender, EventArgs e)
        {
            InvalidateLayout();
        }

        private void ForThemeAndDocument(Action<Document, ThemeBase, ILayout> action)
        {
            if (Document != null && Layout != null && Theme != null)
            {
                action(Document, Theme, Layout);
            }
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            Debug.WriteLine("Arrange");

            ForThemeAndDocument((document, theme, layout) =>
            {
                Layout.UpdateLayout(document, this);

                RenderNodes();
                RenderPaths(theme, finalSize);
            });

            return base.ArrangeOverride(finalSize);
        }

        private void RenderNodes()
        {
            foreach (NodeContainer renderContainer in nodeControls.Values)
            {
                renderContainer.UpdateRenderPosition();
            }

            previewContainer.UpdateRenderPosition();
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

        protected override Size MeasureOverride(Size availableSize)
        {
            ForThemeAndDocument((document, theme, layout) =>
            {
                layout.UpdateVisibility(document, this);

                UpdateStyles(theme);
            });

            availableSize = new Size(double.PositiveInfinity, double.PositiveInfinity);
                
            foreach (UIElement child in Children)
            {
                if (child != null)
                {
                    child.Measure(availableSize);
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
