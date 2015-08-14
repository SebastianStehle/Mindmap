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
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Hercules.App.Components.Implementations;
using Hercules.Model.Rendering.Win2D;
using Hercules.Model.Rendering.Win2D.Default;

namespace Hercules.App.Controls
{
    [TemplatePart(Name = CanvasPart, Type = typeof(CanvasControl))]
    [TemplatePart(Name = TextBoxPart, Type = typeof(TextBox))]
    [TemplatePart(Name = ScrollViewerPart, Type = typeof(ScrollViewer))]
    public sealed class Mindmap : Control
    {
        private const string ScrollViewerPart = "ScrollViewer";
        private const string TextBoxPart = "TextBox";
        private const string CanvasPart = "Canvas";
        private readonly CompositeTransform textBoxTransform = new CompositeTransform();
        private CanvasControl canvasControl;
        private ScrollViewer scrollViewer;
        private TextBox textBox;
        private Win2DRenderNode textEditingNode;

        public Win2DRenderNode TextEditingNode
        {
            get
            {
                return textEditingNode;
            }
        }

        public static readonly DependencyProperty LayoutProperty =
            DependencyProperty.Register("Layout", typeof(ILayout), typeof(Mindmap), new PropertyMetadata(null, OnDocumentLayoutChanged));
        public ILayout Layout
        {
            get { return (ILayout)GetValue(LayoutProperty); }
            set { SetValue(LayoutProperty, value); }
        }

        public static readonly DependencyProperty RendererProperty =
            DependencyProperty.Register("Renderer", typeof(Win2DRenderer), typeof(Mindmap), new PropertyMetadata(null, OnDocumentLayoutChanged));
        public Win2DRenderer Renderer
        {
            get { return (Win2DRenderer)GetValue(RendererProperty); }
            set { SetValue(RendererProperty, value); }
        }

        public static readonly DependencyProperty DocumentProperty =
            DependencyProperty.Register("Document", typeof(Document), typeof(Mindmap), new PropertyMetadata(null, OnDocumentLayoutChanged));
        public Document Document
        {
            get { return (Document)GetValue(DocumentProperty); }
            set { SetValue(DocumentProperty, value); }
        }

        private static void OnDocumentLayoutChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            var owner = o as Mindmap;

            owner?.OnDocumentLayoutChanged();
        }

        private void OnDocumentLayoutChanged()
        {
            if (Renderer != null)
            {
                Renderer.Initialize(Document, Layout, canvasControl);
            }
        }

        public Mindmap()
        {
            DefaultStyleKey = typeof(Mindmap);

            SizeChanged += Mindmap_SizeChanged;
        }

        protected override void OnApplyTemplate()
        {
            canvasControl = GetTemplateChild(CanvasPart) as CanvasControl;

            if (canvasControl != null)
            {
                canvasControl.Draw += CanvasControl_BeforeDraw;
            }

            textBox = GetTemplateChild(TextBoxPart) as TextBox;

            if (textBox != null)
            {
                textBox.GotFocus += TextBox_GotFocus;
                textBox.TextChanged += TextBox_TextChanged;
                textBox.RenderTransform = textBoxTransform;
            }

            scrollViewer = GetTemplateChild(ScrollViewerPart) as ScrollViewer;

            if (scrollViewer != null)
            {
                scrollViewer.ViewChanging += ScrollViewer_ViewChanging;
            }

            if (Renderer != null)
            {
                Renderer.Initialize(Document, Layout, canvasControl);
            }

            if (canvasControl != null)
            {
                canvasControl.Draw += CanvasControl_AfterDraw;
            }
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
            Win2DRenderer renderer = Renderer;

            if (textEditingNode != null && textEditingNode.Node.Document != null && renderer != null)
            {
                Vector2 position = Renderer.GetOverlayPosition(textEditingNode.TextRenderer.RenderPosition);
                
                textBoxTransform.TranslateX = position.X;
                textBoxTransform.TranslateY = position.Y;

                textBoxTransform.ScaleX = renderer.ZoomFactor;
                textBoxTransform.ScaleY = renderer.ZoomFactor;

                textBox.Height = textEditingNode.TextRenderer.RenderSize.Y;

                textBox.Width = textEditingNode.TextRenderer.RenderSize.X;
            }
        }

        private void CanvasControl_BeforeDraw(CanvasControl sender, CanvasDrawEventArgs args)
        {
            Win2DRenderer renderer = Renderer;

            if (Document != null && scrollViewer != null && renderer != null)
            {
                double zoomFactor = scrollViewer.ZoomFactor;

                double vOffset = scrollViewer.VerticalOffset;
                double hOffset = scrollViewer.HorizontalOffset;

                double inverseZoom = 1f / zoomFactor;

                double scaledContentW = Document.Size.X * zoomFactor;
                double scaledContentH = Document.Size.Y * zoomFactor;

                double translateX;
                double translateY;

                if (scaledContentW < scrollViewer.ViewportWidth)
                {
                    translateX = (scrollViewer.ViewportWidth * inverseZoom - Document.Size.X) * 0.5;
                }
                else
                {
                    translateX = -inverseZoom * scrollViewer.HorizontalOffset;
                }

                if (scaledContentH < scrollViewer.ViewportHeight)
                {
                    translateY = (scrollViewer.ViewportHeight * inverseZoom - Document.Size.Y) * 0.5;
                }
                else
                {
                    translateY = -inverseZoom * vOffset;
                }

                double visibleX = inverseZoom * hOffset;
                double visibleY = inverseZoom * vOffset;

                double visibleW = Math.Min(Document.Size.X, inverseZoom * scrollViewer.ViewportWidth);
                double visibleH = Math.Min(Document.Size.Y, inverseZoom * scrollViewer.ViewportHeight);

                Rect2 visibleRect = new Rect2((float)visibleX, (float)visibleY, (float)visibleW, (float)visibleH);

                renderer.Transform(new Vector2((float)translateX, (float)translateY), (float)zoomFactor, visibleRect);
            }
        }

        private void Mindmap_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            Win2DRenderer renderer = Renderer;

            if (renderer != null)
            {
                Renderer.Invalidate();
            }
        }

        private void ScrollViewer_ViewChanging(object sender, ScrollViewerViewChangingEventArgs e)
        {
            Win2DRenderer renderer = Renderer;

            if (renderer != null)
            {
                Renderer.Invalidate();
            }
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            Win2DRenderer renderer = Renderer;

            if (renderer != null)
            {
                Renderer.Invalidate();
            }
        }

        protected override void OnPointerPressed(PointerRoutedEventArgs e)
        {
            e.Handled = true;
        }

        protected override void OnPointerReleased(PointerRoutedEventArgs e)
        {
            e.Handled = true;
        }

        protected override void OnDoubleTapped(DoubleTappedRoutedEventArgs e)
        {
            Win2DRenderer renderer = Renderer;

            if (renderer != null)
            {
                Vector2 position = renderer.GetMindmapPosition(e.GetPosition(this).ToVector2());

                foreach (Win2DRenderNode renderNode in renderer.RenderNodes)
                {
                    if (renderNode.HitTest(position))
                    {
                        if (textEditingNode != renderNode)
                        {
                            FinishTextEditing();

                            textEditingNode = renderNode;
                            textEditingNode.TextRenderer.HideText = true;

                            textBox.Text = renderNode.Node.Text ?? string.Empty;
                            textBox.FontSize = renderNode.TextRenderer.FontSize;
                            textBox.Visibility = Visibility.Visible;

                            textBox.Focus(FocusState.Pointer);
                        }

                        renderer.Invalidate();

                        e.Handled = true;
                        break;
                    }
                }
            }
        }

        protected override void OnTapped(TappedRoutedEventArgs e)
        {
            Win2DRenderer renderer = Renderer;

            if (renderer != null)
            {
                Vector2 position = renderer.GetMindmapPosition(e.GetPosition(this).ToVector2());

                Win2DRenderNode handledNode;

                if (renderer.HandleClick(position, out handledNode))
                {
                    if (textEditingNode == handledNode)
                    {
                        textBox.Focus(FocusState.Pointer);
                    }
                    else
                    {
                        FinishTextEditing();
                    }

                    e.Handled = true;
                }
                else
                {
                    FinishTextEditing();
                }

                base.OnTapped(e);
            }
        }

        private void FinishTextEditing()
        {
            Win2DRenderer renderer = Renderer;

            if (renderer != null)
            {
                if (textEditingNode != null)
                {
                    textEditingNode.TextRenderer.HideText = false;

                    string transactionName = ResourceManager.GetString("ChangeTextTransactionName");

                    textEditingNode.Node.Document.MakeTransaction(transactionName, commands =>
                    {
                        commands.Apply(new ChangeTextCommand(textEditingNode.Node, textBox.Text, true));
                    });

                    textBox.Visibility = Visibility.Collapsed;

                    textEditingNode = null;
                }

                renderer.Invalidate();
            }
        }
    }
}
