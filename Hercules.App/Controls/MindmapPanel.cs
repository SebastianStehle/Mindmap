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
        private readonly CompositeTransform textBoxTransform = new CompositeTransform();
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
            textBox.GotFocus += TextBox_GotFocus;
            textBox.TextChanged += TextBox_TextChanged;

            scrollViewer = (ScrollViewer)GetTemplateChild("ScrollViewer");
            scrollViewer.SizeChanged += ScrollViewer_SizeChanged;
            scrollViewer.ViewChanging += ScrollViewer_ViewChanging;
            scrollViewer.CenterViewport();

            renderer.Initialize(Document, Layout, canvasControl);

            canvasControl.Draw += CanvasControl_AfterDraw;
        }

        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(textBox.Text))
            {
                textBox.Select(textBox.Text.Length, 1);
            }
        }

        private void CanvasControl_AfterDraw(CanvasControl sender, CanvasDrawEventArgs args)
        {
            if (textBoxNode != null && textBoxNode.Node.Document != null)
            {
                Vector2 position = renderer.GetOverlayPosition(textBoxNode.TextRenderer.Position);

                Vector2 size = renderer.GetOverlaySize(textBoxNode.TextRenderer.Size);
                
                textBoxTransform.TranslateX = position.X;
                textBoxTransform.TranslateY = position.Y;
                textBoxTransform.ScaleX = renderer.ZoomFactor;
                textBoxTransform.ScaleY = renderer.ZoomFactor;

                textBox.Height = textBoxNode.TextRenderer.Size.Y;
                textBox.Width = textBoxNode.TextRenderer.Size.X;
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

                float visibleX = (float)Math.Round(inverseZoom * (float)scrollViewer.HorizontalOffset, 3);
                float visibleY = (float)Math.Round(inverseZoom * (float)scrollViewer.VerticalOffset, 3);

                float visibleW = (float)Math.Round(Math.Min(Document.Size.X, inverseZoom * (float)scrollViewer.ViewportWidth), 3);
                float visibleH = (float)Math.Round(Math.Min(Document.Size.Y, inverseZoom * (float)scrollViewer.ViewportHeight), 3);

                Rect2 visibleRect = new Rect2(visibleX, visibleY, visibleW, visibleH);

                translateX = (float)Math.Round(translateX, 3);
                translateY = (float)Math.Round(translateY, 3);

                float zoom = (float)Math.Round(scrollViewer.ZoomFactor, 3);

                renderer.Transform(new Vector2(translateX, translateY), zoom, visibleRect);
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

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            canvasControl.Invalidate();
        }

        protected override void OnGotFocus(RoutedEventArgs e)
        {
            if (textBoxNode != null)
            {
                //textBox.Focus(FocusState.Pointer);
            }
        }

        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            var a = FocusManager.GetFocusedElement();
        }

        protected override void OnPointerPressed(PointerRoutedEventArgs e)
        {
            e.Handled = true;
            base.OnPointerPressed(e);
        }

        protected override void OnPointerReleased(PointerRoutedEventArgs e)
        {
            e.Handled = true;
            base.OnPointerReleased(e);
        }

        protected override void OnDoubleTapped(DoubleTappedRoutedEventArgs e)
        {
            Vector2 position = renderer.GetMindmapPosition(e.GetPosition(this).ToVector2());

            foreach (ThemeRenderNode renderNode in renderer.RenderNodes)
            {
                if (renderNode.HitTest(position))
                {
                    if (textBoxNode != renderNode)
                    {
                        FinishTextEditing();

                        textBoxNode = renderNode;

                        textBoxNode.TextRenderer.BeginEdit(textBox);

                        textBox.Visibility = Visibility.Visible;

                        textBox.Focus(FocusState.Pointer);
                    }

                    if (canvasControl != null)
                    {
                        canvasControl.Invalidate();
                    }

                    // Focus(FocusState.Programmatic);

                    e.Handled = true;
                    break;
                }
            }
        }

        protected override void OnTapped(TappedRoutedEventArgs e)
        {
            Vector2 position = renderer.GetMindmapPosition(e.GetPosition(this).ToVector2());

            ThemeRenderNode handledNode = null;

            if (renderer.HandleClick(position, out handledNode))
            {
                if (textBoxNode == handledNode)
                {
                    textBox.Focus(FocusState.Pointer);
                }

                e.Handled = true;
            }
            else
            {
                FinishTextEditing();
            }

            base.OnTapped(e);
        }

        private void FinishTextEditing()
        {
            if (textBoxNode != null)
            {
                textBoxNode.TextRenderer.EndEdit();

                textBoxNode.Node.Document.MakeTransaction("Change Text", c =>
                {
                    c.Apply(new ChangeTextCommand(textBoxNode.Node, textBox.Text, true));
                });

                textBox.Visibility = Visibility.Collapsed;

                textBoxNode = null;
            }

            if (canvasControl != null)
            {
                canvasControl.Invalidate();
            }
        }
    }
}
