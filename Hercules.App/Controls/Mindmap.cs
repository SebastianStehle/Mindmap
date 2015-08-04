// ==========================================================================
// Mindmap.cs
// Mindmap Application
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using Hercules.Model;
using Hercules.Model.Layouting;
using GP.Windows.UI;
using GP.Windows.UI.Controls;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Core;

namespace Hercules.App.Controls
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
#pragma warning disable 4014
            Dispatcher.RunAsync(CoreDispatcherPriority.High, () =>
            {
                
            });
#pragma warning restore 4014
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

            nodePanel.SizeChanged += NodePanel_SizeChanged;
        }

        private void NodePanel_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (scrollViewer != null)
            {
                scrollViewer.CenterViewport();
            }
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
    }
}
