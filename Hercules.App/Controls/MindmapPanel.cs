// ==========================================================================
// Mindmap.cs
// Mindmap Application
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System;
using System.Numerics;
using GP.Windows.UI;
using Hercules.Model;
using Hercules.Model.Layouting;
using Hercules.Model.Utils;
using Microsoft.Graphics.Canvas.UI.Xaml;
using Windows.UI.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Hercules.App.Controls.Default;
using Windows.UI.Xaml.Media;

namespace Hercules.App.Controls
{
    public sealed class MindmapPanel : Control
    {
        private readonly ThemeRenderer renderer = new DefaultRenderer();
        private readonly TranslateTransform textBoxTransform = new TranslateTransform();
        private CanvasControl canvasControl;
        private FrameworkElement placeholder;
        private ScrollViewer scrollViewer;
        private TextBox textBox;
        private ThemeRenderNode textBoxNode;

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
            renderer.Initialize(Document, Layout, canvasControl);
        }

        public MindmapPanel()
        {
            DefaultStyleKey = typeof(MindmapPanel);
        }

        protected override void OnApplyTemplate()
        {
            canvasControl = (CanvasControl)GetTemplateChild("Canvas");
            canvasControl.Draw += CanvasControl_BeforeDraw;

            placeholder = (FrameworkElement)GetTemplateChild("Placeholder");
            placeholder.SizeChanged += ScrollViewer_SizeChanged;

            textBox = (TextBox)GetTemplateChild("TextBox");
            textBox.RenderTransform = textBoxTransform;
            textBox.LostFocus += TextBox_LostFocus;

            scrollViewer = (ScrollViewer)GetTemplateChild("ScrollViewer");
            scrollViewer.SizeChanged += ScrollViewer_SizeChanged;
            scrollViewer.ViewChanging += ScrollViewer_ViewChanging;
            scrollViewer.CenterViewport();

            renderer.Initialize(Document, Layout, canvasControl);

            canvasControl.Draw += CanvasControl_AfterDraw;
        }

        private void CanvasControl_AfterDraw(CanvasControl sender, CanvasDrawEventArgs args)
        {
            if (textBoxNode != null && textBoxNode.Node.Document != null)
            {
                Vector2 position = renderer.GetOverlayPosition(textBoxNode.Position);

                textBox.Visibility = Visibility.Visible;

                textBoxTransform.X = position.X;
                textBoxTransform.Y = position.Y;

                textBox.Focus(FocusState.Programmatic);
            }
            else
            {
                textBox.Visibility = Visibility.Collapsed;
            }
        }

        private void CanvasControl_BeforeDraw(CanvasControl sender, CanvasDrawEventArgs args)
        {
            if (Document != null && scrollViewer != null)
            {
                float inverseZoom = 1f / scrollViewer.ZoomFactor;

                float scaledContentW = Document.Size.X * scrollViewer.ZoomFactor;
                float scaledContentH = Document.Size.Y * scrollViewer.ZoomFactor;

                float translateX;
                float translateY;

                if (scaledContentW < scrollViewer.ViewportWidth)
                {
                    translateX = ((float)scrollViewer.ViewportWidth * inverseZoom - Document.Size.X) * 0.5f;
                }
                else
                {
                    translateX = -inverseZoom * (float)scrollViewer.HorizontalOffset;
                }

                if (scaledContentH < scrollViewer.ViewportHeight)
                {
                    translateY = ((float)scrollViewer.ViewportHeight * inverseZoom - Document.Size.Y) * 0.5f;
                }
                else
                {
                    translateY = -inverseZoom * (float)scrollViewer.VerticalOffset;
                }

                float visibleX = inverseZoom * (float)scrollViewer.HorizontalOffset;
                float visibleY = inverseZoom * (float)scrollViewer.VerticalOffset;

                float visibleW = Math.Min(Document.Size.X, inverseZoom * (float)scrollViewer.ViewportWidth);
                float visibleH = Math.Min(Document.Size.Y, inverseZoom * (float)scrollViewer.ViewportHeight);

                Rect2 visibleRect = new Rect2(visibleX, visibleY, visibleW, visibleH);

                renderer.Transform(new Vector2(translateX, translateY), scrollViewer.ZoomFactor, visibleRect);
            }
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

        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            
        }

        protected override void OnDoubleTapped(DoubleTappedRoutedEventArgs e)
        {
            Vector2 position = renderer.GetMindmapPosition(e.GetPosition(this).ToVector2());

            foreach (ThemeRenderNode renderNode in renderer.RenderNodes)
            {
                if (renderNode.HitTest(position))
                {
                    textBoxNode = renderNode;

                    textBox.Text = renderNode.Node.Text ?? string.Empty;

                    if (canvasControl != null)
                    {
                        canvasControl.Invalidate();
                    }

                    break;
                }
            }

            base.OnDoubleTapped(e);
        }

        protected override void OnTapped(TappedRoutedEventArgs e)
        {
            Vector2 position = renderer.GetMindmapPosition(e.GetPosition(this).ToVector2());

            if (!renderer.HandleClick(position))
            {
                if (textBoxNode != null)
                {
                    textBoxNode = null;

                    if (canvasControl != null)
                    {
                        canvasControl.Invalidate();
                    }
                }
            }

            base.OnTapped(e);
        }

        private Vector2 GetMindmapPosition(PointerPoint point)
        {
            Vector2 position = new Vector2((float)point.Position.X, (float)point.Position.Y);

            position = renderer.GetMindmapPosition(position);

            return position;
        }
    }
}
