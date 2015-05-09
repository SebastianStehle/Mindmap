// ==========================================================================
// Mindmap.cs
// RavenMind Application
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using RavenMind.Model;
using SE.Metro.UI;
using SE.Metro.UI.Controls;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace RavenMind.Controls
{
    [TemplatePart(Name = PartScrollViewer, Type = typeof(ScrollViewer))]
    [TemplatePart(Name = PartPathsCanvas, Type = typeof(Canvas))]
    [TemplatePart(Name = PartNodesCanvas, Type = typeof(Canvas))]
    public class Mindmap : LoadableControl
    {
        #region Constants

        private const string PartScrollViewer = "ScrollViewer";
        private const string PartPathsCanvas = "PathsCanvas";
        private const string PartNodesCanvas = "NodesCanvas";

        #endregion

        #region Fields

        private readonly Dictionary<NodeBase, NodePath> nodePaths = new Dictionary<NodeBase, NodePath>();
        private readonly Dictionary<NodeBase, NodeControl> nodeControls = new Dictionary<NodeBase, NodeControl>();
        private Canvas pathsCanvas;
        private Canvas nodesCanvas;
        private ScrollViewer scrollViewer;
        private int transactionCount;
        private bool isMindmapLayoutingRequired;

        #endregion

        #region Properties

        public Canvas PathsCanvas
        {
            get { return pathsCanvas; }
        }

        public Canvas NodesCanvas
        {
            get { return nodesCanvas; }
        }

        public ScrollViewer ScrollViewer
        {
            get { return scrollViewer; }
        }

        #endregion

        #region Dependency Properties

        public static readonly DependencyProperty LayoutProperty =
            DependencyProperty.Register("Layout", typeof(ILayout), typeof(Mindmap), new PropertyMetadata(null));
        public ILayout Layout
        {
            get { return (ILayout)GetValue(LayoutProperty); }
            set { SetValue(LayoutProperty, value); }
        }

        public static readonly DependencyProperty RootStyleProperty =
            DependencyProperty.Register("RootStyle", typeof(Style), typeof(Mindmap), new PropertyMetadata(null));
        public Style RootStyle
        {
            get { return (Style)GetValue(RootStyleProperty); }
            set { SetValue(RootStyleProperty, value); }
        }

        public static readonly DependencyProperty NodeStyleProperty =
            DependencyProperty.Register("NodeStyle", typeof(Style), typeof(Mindmap), new PropertyMetadata(null));
        public Style NodeStyle
        {
            get { return (Style)GetValue(NodeStyleProperty); }
            set { SetValue(NodeStyleProperty, value); }
        }

        public static readonly DependencyProperty DocumentProperty =
            DependencyProperty.Register("Document", typeof(Document), typeof(Mindmap), new PropertyMetadata(null, new PropertyChangedCallback(OnDocumentChanged)));
        public Document Document
        {
            get { return (Document)GetValue(DocumentProperty); }
            set { SetValue(DocumentProperty, value); }
        }

        private static void OnDocumentChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            var owner = o as Mindmap;
            if (owner != null)
            {
                owner.OnDocumentChanged(e);
            }
        }

        private void OnDocumentChanged(DependencyPropertyChangedEventArgs e)
        {
            MakeLayoutTransactional(() =>
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

        #endregion

        #region Constructors

        public Mindmap()
        {
            DefaultStyleKey = typeof(Mindmap);

            SizeChanged += Mindmap_SizeChanged;

            LayoutUpdated += Mindmap_LayoutUpdated;
        }

        #endregion

        #region Methods

        private void HandleDocumentAdded(Document newDocument)
        {
            newDocument.UndoRedoManager.StateChanged += UndoRedoManager_StateChanged;
            newDocument.NodeAdded += oldDocument_NodeAdded;
            newDocument.NodeRemoved += oldDocument_NodeRemoved;

            foreach (NodeBase node in newDocument.Nodes)
            {
                HandleNodeAdded(node);
            }

            if (scrollViewer != null)
            {
                scrollViewer.CenterViewport();
            }
        }

        private void HandleDocumentRemoved(Document oldDocument)
        {
            oldDocument.UndoRedoManager.StateChanged -= UndoRedoManager_StateChanged;
            oldDocument.NodeAdded -= oldDocument_NodeAdded;
            oldDocument.NodeRemoved -= oldDocument_NodeRemoved;

            foreach (NodeBase node in nodeControls.Keys.ToList())
            {
                HandleNodeRemoved(node);
            }
        }

        private void oldDocument_NodeRemoved(object sender, CollectionItemEventArgs<NodeBase> e)
        {
            HandleNodeRemoved(e.Item);
        }

        private void oldDocument_NodeAdded(object sender, CollectionItemEventArgs<NodeBase> e)
        {
            HandleNodeAdded(e.Item);
        }

        private void HandleNodeAdded(NodeBase node)
        {
            NodeControl nodeControl = new NodeControl();
            nodeControl.Attach(node);
            nodeControl.SizeChanged += nodeControl_SizeChanged;
            nodeControls[node] = nodeControl;

            VisualTreeExtensions.TryAdd(nodesCanvas, nodeControl);

            Node normalNode = node as Node;

            if (normalNode != null)
            {
                nodeControl.Style = NodeStyle;

                NodePath nodePath = new NodePath();
                nodePath.DataContext = node;
                nodePath.BindEvents(nodeControls[node.Parent], nodeControl);
                nodePaths[node] = nodePath;

                VisualTreeExtensions.TryAdd(pathsCanvas, nodePath);
            }
            else
            {
                nodeControl.Style = RootStyle;
            }
        }

        private void HandleNodeRemoved(NodeBase node)
        {
            NodeControl nodeControl = nodeControls[node];
            nodeControl.SizeChanged -= nodeControl_SizeChanged;
            nodeControl.Detach();
            nodeControls.Remove(node);

            VisualTreeExtensions.TryRemove(nodesCanvas, nodeControl);

            Node normalNode = node as Node;

            if (normalNode != null)
            {
                NodePath nodePath = nodePaths[normalNode];
                nodePath.DataContext = null;
                nodePath.UnbindEvents();
                nodePaths.Remove(node);

                VisualTreeExtensions.TryRemove(pathsCanvas, nodePath);
            }
        }

        private void UndoRedoManager_StateChanged(object sender, EventArgs e)
        {
            MakeLayoutUpdate();
        }

        private void collectionChanged_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Reset)
            {
                foreach (Node node in nodeControls.Keys.ToList())
                {
                    HandleNodeRemoved(node);
                }
            }
            else
            {
                if (e.OldItems != null)
                {
                    foreach (NodeBase node in e.OldItems)
                    {
                        HandleNodeRemoved(node);
                    }
                }

                if (e.NewItems != null)
                {
                    foreach (NodeBase node in e.NewItems)
                    {
                        HandleNodeAdded(node);
                    }
                }
            }
        }

        protected override void OnApplyTemplate()
        {
            scrollViewer = (ScrollViewer)GetTemplateChild(PartScrollViewer);

            nodesCanvas = (Canvas)GetTemplateChild(PartNodesCanvas);
            pathsCanvas = (Canvas)GetTemplateChild(PartPathsCanvas);
            
            foreach (NodeControl nodeControl in nodeControls.Values)
            {
                nodesCanvas.Children.Add(nodeControl);
            }

            foreach (NodePath nodePath in nodePaths.Values)
            {
                pathsCanvas.Children.Add(nodePath);
            }
        }

        private void Mindmap_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (scrollViewer != null)
            {
                scrollViewer.CenterViewport();
            }
        }

        private void nodeControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (Document != null && !Document.IsChangeTracking)
            {
                isMindmapLayoutingRequired = true;
            }
        }

        private void Mindmap_LayoutUpdated(object sender, object e)
        {
            if (Document != null && nodesCanvas != null && Layout != null && isMindmapLayoutingRequired)
            {
                Layout.IsAnimating = true;
                Layout.UpdateLayout(Document, GetControlView, nodesCanvas.RenderSize);
                Layout.IsAnimating = false;

                isMindmapLayoutingRequired = false;
            }
        }

        private void MakeLayoutUpdate()
        {
            if (Document != null && nodesCanvas != null && Layout != null)
            {
#pragma warning disable 4014
                Dispatcher.RunIdleAsync(x =>
                   {
                       Layout.IsAnimating = true;
                       Layout.UpdateLayout(Document, GetControlView, nodesCanvas.RenderSize);
                       Layout.IsAnimating = false;
                   });
#pragma warning restore 4014
            }
        }

        private void MakeLayoutTransactional(Action action)
        {
            try
            {
                transactionCount++;
                action();
            }
            finally
            {
                transactionCount--;

                if (transactionCount == 0)
                {
                    isMindmapLayoutingRequired = true;
                }
            }
        }

        public NodePath GetPath(Node node)
        {
            return nodePaths[node];
        }

        public INodeView GetControlView(NodeBase node)
        {
            return nodeControls[node];
        }

        public NodeControl GetControl(Node node)
        {
            return nodeControls[node];
        }

        public void StayAtSide(Node node)
        {
            NodeControl element = GetControl(node);

            Point position = element.Position;

            if (position.X + (0.5 * element.RenderSize.Width) < (nodesCanvas.RenderSize.Width * 0.5))
            {
                foreach (Node child in node.AllChildren())
                {
                    ChangeSide(child, element, position, true);
                }
            }
            else
            {
                foreach (Node child in node.AllChildren())
                {
                    ChangeSide(child, element, position, false);
                }
            }
        }

        private void ChangeSide(Node node, UIElement parentElement, Point parentPosition, bool left)
        {
            NodeControl element = GetControl(node);

            Point position = element.Position;

            if (position.X < parentPosition.X && !left)
            {
                position.X = parentPosition.X + Math.Abs(parentPosition.X - position.X) + parentElement.RenderSize.Width - element.RenderSize.Width;

                element.SetPosition(position, false);
            }
            else if (position.X > parentPosition.X && left)
            {
                position.X = parentPosition.X - Math.Abs(position.X - parentPosition.X) + parentElement.RenderSize.Width - element.RenderSize.Width;

                element.SetPosition(position, false);
            }
        }

        #endregion
    }
}
