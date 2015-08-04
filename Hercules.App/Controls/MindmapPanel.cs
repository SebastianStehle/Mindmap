// ==========================================================================
// Mindmap.cs
// Mindmap Application
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using Hercules.Model;
using Hercules.Model.Layouting;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Microsoft.Graphics.Canvas.UI.Xaml;
using Windows.UI.Xaml.Input;
using Windows.UI.Input;
using Hercules.App.Controls.Default;
using System.Numerics;
using Microsoft.Graphics.Canvas;
using Hercules.Model.Utils;
using GP.Windows.UI;
using System.Diagnostics;
using Windows.UI;
using System;

namespace Hercules.App.Controls
{
    public sealed class MindmapPanel : Control
    {
        private readonly ThemeRenderer renderer = new DefaultRenderer();
        private CanvasControl canvasControl;
        private FrameworkElement placeholder;
        private ScrollViewer scrollViewer;
        private Matrix3x2 transform;

        public static readonly DependencyProperty LayoutProperty =
            DependencyProperty.Register("Layout", typeof(ILayout), typeof(MindmapPanel), new PropertyMetadata(null, new PropertyChangedCallback(OnDocumentLayoutChanged)));
        public ILayout Layout
        {
            get { return (ILayout)GetValue(LayoutProperty); }
            set { SetValue(LayoutProperty, value); }
        }

        public static readonly DependencyProperty DocumentProperty =
            DependencyProperty.Register("Document", typeof(Document), typeof(MindmapPanel), new PropertyMetadata(null, new PropertyChangedCallback(OnDocumentLayoutChanged)));
        public Document Document
        {
            get { return (Document)GetValue(DocumentProperty); }
            set { SetValue(DocumentProperty, value); }
        }

        private static void OnDocumentLayoutChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            var owner = o as MindmapPanel;
            if (owner != null)
            {
                owner.OnDocumentLayoutChanged(e);
            }
        }

        private void OnDocumentLayoutChanged(DependencyPropertyChangedEventArgs e)
        {
            renderer.Initialize(Document, Layout);
        }

        public MindmapPanel()
        {
            DefaultStyleKey = typeof(MindmapPanel);
        }

        protected override void OnApplyTemplate()
        {
            canvasControl = (CanvasControl)GetTemplateChild("Canvas");
            canvasControl.Draw += CanvasControl_Draw;

            placeholder = (FrameworkElement)GetTemplateChild("Placeholder");
            placeholder.SizeChanged += ScrollViewer_SizeChanged;

            scrollViewer = (ScrollViewer)GetTemplateChild("ScrollViewer");
            scrollViewer.SizeChanged += ScrollViewer_SizeChanged;
            scrollViewer.ViewChanging += ScrollViewer_ViewChanging;
            scrollViewer.CenterViewport();
        }

        private void ScrollViewer_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            scrollViewer.CenterViewport();

            canvasControl.Invalidate();
        }

        private void ScrollViewer_ViewChanging(object sender, ScrollViewerViewChangingEventArgs e)
        {
            canvasControl.Invalidate();
        }

        private void CanvasControl_Draw(CanvasControl sender, CanvasDrawEventArgs args)
        {
            CanvasDrawingSession session = args.DrawingSession;

            if (Document != null && session != null)
            {
                float inverseZoom = 1f / (float)scrollViewer.ZoomFactor;

                float scaledContentW = Document.Size.X * scrollViewer.ZoomFactor;
                float scaledContentH = Document.Size.Y * scrollViewer.ZoomFactor;

                float transformX = 0;
                float transformY;

                if (scaledContentW < scrollViewer.ViewportWidth)
                {
                    transformX = ((float)scrollViewer.ViewportWidth * inverseZoom - Document.Size.X) * 0.5f;
                }
                else
                {
                    transformX = -inverseZoom * (float)scrollViewer.HorizontalOffset;
                }

                if (scaledContentH < scrollViewer.ViewportHeight)
                {
                    transformY = ((float)scrollViewer.ViewportHeight * inverseZoom - Document.Size.Y) * 0.5f;
                }
                else
                {
                    transformY = -inverseZoom * (float)scrollViewer.VerticalOffset;
                }

                Rect2 bounds = new Rect2(
                    inverseZoom * (float)scrollViewer.HorizontalOffset,
                    inverseZoom * (float)scrollViewer.VerticalOffset,
                    Math.Min(Document.Size.X, inverseZoom * (float)scrollViewer.ViewportWidth),
                    Math.Min(Document.Size.Y, inverseZoom * (float)scrollViewer.ViewportHeight));
                
                transform =
                    Matrix3x2.CreateTranslation(
                        transformX,
                        transformY) *
                    Matrix3x2.CreateScale(scrollViewer.ZoomFactor);

                session.Transform = transform;

                renderer.Render(session, bounds);
            }
        }

        protected override void OnPointerPressed(PointerRoutedEventArgs e)
        {
            PointerPoint point = e.GetCurrentPoint(this);

            Vector2 position = new Vector2((float)point.Position.X, (float)point.Position.Y);

            position = transform * position;

            foreach (ThemeRenderNode renderNode in renderer.RenderNodes)
            {
                if (renderNode.HitTest(position))
                {
                    renderNode.Node.Select();

                    canvasControl.Invalidate();
                    break;
                }
            }

            base.OnPointerPressed(e);
        }
    }
}
