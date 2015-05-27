// ==========================================================================
// Mindmap.cs
// Mindmap Application
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using Mindmap.Model;
using Mindmap.Model.Layouting;
using SE.Metro.UI;
using SE.Metro.UI.Controls;
using System;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Mindmap.Controls
{
    [TemplatePart(Name = PartScrollViewer, Type = typeof(ScrollViewer))]
    [TemplatePart(Name = PartAdornerLayer, Type = typeof(Canvas))]
    [TemplatePart(Name = PartNodePanel, Type = typeof(MindmapPanel))]
    public class Mindmap : LoadableControl
    {
        private const double AnimationSpeed = 800;
        private const string PartScrollViewer = "ScrollViewer";
        private const string PartAdornerLayer = "AdornerLayer";
        private const string PartNodePanel = "NodePanel";

        private MindmapPanel nodePanel;
        private Canvas adornerLayer;
        private ScrollViewer scrollViewer;

        public Panel NodePanel
        {
            get
            {
                return nodePanel;
            }
        }

        public ScrollViewer ScrollViewer
        {
            get
            {
                return scrollViewer;
            }
        }

        public static readonly DependencyProperty LayoutProperty =
            DependencyProperty.Register("Layout", typeof(ILayout), typeof(Mindmap), new PropertyMetadata(null));
        public ILayout Layout
        {
            get { return (ILayout)GetValue(LayoutProperty); }
            set { SetValue(LayoutProperty, value); }
        }

        public static readonly DependencyProperty ThemeProperty =
            DependencyProperty.Register("Theme", typeof(ThemeBase), typeof(Mindmap), new PropertyMetadata(null));
        public ThemeBase Theme
        {
            get { return (ThemeBase)GetValue(ThemeProperty); }
            set { SetValue(ThemeProperty, value); }
        }

        public static readonly DependencyProperty DocumentProperty =
            DependencyProperty.Register("Document", typeof(Document), typeof(Mindmap), new PropertyMetadata(null));
        public Document Document
        {
            get { return (Document)GetValue(DocumentProperty); }
            set { SetValue(DocumentProperty, value); }
        }
        
        public Mindmap()
        {
            DefaultStyleKey = typeof(Mindmap);

            SizeChanged += Mindmap_SizeChanged;
        }

        protected override void OnApplyTemplate()
        {
            scrollViewer = (ScrollViewer)GetTemplateChild(PartScrollViewer);

            adornerLayer = (Canvas)GetTemplateChild(PartAdornerLayer);

            nodePanel = (MindmapPanel)GetTemplateChild(PartNodePanel);
        }

        private void Mindmap_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (scrollViewer != null)
            {
                scrollViewer.CenterViewport();
            }
        }

        public void AddAdorner(UIElement adorner)
        {
            if (adornerLayer != null)
            {
                adornerLayer.Children.Add(adorner);
            }
        }

        public void ClearAdorners()
        {
            if (adornerLayer != null)
            {
                adornerLayer.Children.Clear();
            }
        }

        public void RemoveAdorner(UIElement adorner)
        {
            if (adornerLayer != null)
            {
                adornerLayer.Children.Remove(adorner);
            }
        }

        public void ShowPreviewElement(Point? position, NodeBase parent, AnchorPoint anchor)
        {
            nodePanel.ShowPreviewElement(position, parent, anchor);
        }

        public AttachTarget CalculateAttachTarget(Node movingNode, Rect movementBounds)
        {
            return Layout.CalculateAttachTarget(Document, nodePanel, movingNode, movementBounds);
        }

        public Rect GetBounds(NodeBase node)
        {
            return nodePanel.GetBounds(node);
        }

        public NodeControl GetControl(NodeBase node)
        {
            return nodePanel.GetControl(node);
        }
    }
}
