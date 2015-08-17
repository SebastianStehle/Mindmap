// ==========================================================================
// Mindmap.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================
using System;
using System.Numerics;
using Hercules.Model;
using Hercules.Model.Layouting;
using Hercules.Model.Utils;
using Microsoft.Graphics.Canvas.UI.Xaml;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Hercules.Model.Rendering.Win2D;

// ReSharper disable UnusedParameter.Local

namespace Hercules.App.Controls
{
    [TemplatePart(Name = CanvasPart, Type = typeof(CanvasControl))]
    [TemplatePart(Name = TextBoxPart, Type = typeof(TextEditor))]
    [TemplatePart(Name = ScrollViewerPart, Type = typeof(ScrollViewer))]
    public sealed class Mindmap : Control
    {
        private const string ScrollViewerPart = "ScrollViewer";
        private const string TextBoxPart = "TextBox";
        private const string CanvasPart = "Canvas";
        private IRendererFactory lastRendererFactory;
        private Win2DRenderer renderer;
        private CanvasControl canvasControl;
        private ScrollViewer scrollViewer;
        private TextEditor textEditor;

        public Win2DRenderNode TextEditingNode
        {
            get
            {
                return textEditor.EditingNode;
            }
        }

        public Win2DRenderer Renderer
        {
            get
            {
                return renderer;
            }
        }

        public static readonly DependencyProperty LayoutProperty =
            DependencyProperty.Register("Layout", typeof(ILayout), typeof(Mindmap), new PropertyMetadata(null, OnDocumentRendererLayoutChanged));
        public ILayout Layout
        {
            get { return (ILayout)GetValue(LayoutProperty); }
            set { SetValue(LayoutProperty, value); }
        }

        public static readonly DependencyProperty RendererFactoryProperty =
            DependencyProperty.Register("RendererFactory", typeof(IRendererFactory), typeof(Mindmap), new PropertyMetadata(null, OnDocumentRendererLayoutChanged));
        public IRendererFactory RendererFactory
        {
            get { return (IRendererFactory)GetValue(RendererFactoryProperty); }
            set { SetValue(RendererFactoryProperty, value); }
        }

        public static readonly DependencyProperty DocumentProperty =
            DependencyProperty.Register("Document", typeof(Document), typeof(Mindmap), new PropertyMetadata(null, OnDocumentRendererLayoutChanged));
        public Document Document
        {
            get { return (Document)GetValue(DocumentProperty); }
            set { SetValue(DocumentProperty, value); }
        }

        private static void OnDocumentRendererLayoutChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            var owner = o as Mindmap;

            owner?.OnDocumentRendererLayoutChanged();
        }

        private void OnDocumentRendererLayoutChanged()
        {
            InitializeRenderer();
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

            textEditor = GetTemplateChild(TextBoxPart) as TextEditor;

            scrollViewer = GetTemplateChild(ScrollViewerPart) as ScrollViewer;

            if (scrollViewer != null)
            {
                scrollViewer.ViewChanging += ScrollViewer_ViewChanging;
            }

            InitializeRenderer();
        }

        private void InitializeRenderer()
        {
            if (canvasControl != null && Document != null && RendererFactory != null)
            {
                if (renderer == null || renderer.Document != Document || renderer.Canvas != canvasControl || RendererFactory != lastRendererFactory)
                {
                    renderer = RendererFactory.CreateRenderer(Document, canvasControl);

                    lastRendererFactory = RendererFactory;
                }
            }

            if (renderer != null && Layout != null)
            {
                renderer.Layout = Layout;
            }
        }

        private void CanvasControl_BeforeDraw(CanvasControl sender, CanvasDrawEventArgs args)
        {
            WithRenderer(renderer =>
            {
                if (scrollViewer != null)
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

                if (textEditor != null)
                {
                    textEditor.Transform(renderer);
                }
            });
        }

        private void Mindmap_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            WithRenderer(renderer => renderer.Invalidate());
        }

        private void ScrollViewer_ViewChanging(object sender, ScrollViewerViewChangingEventArgs e)
        {
            WithRenderer(renderer => renderer.Invalidate());
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
            WithRenderer(renderer =>
            {
                Vector2 position = renderer.GetMindmapPosition(e.GetPosition(this).ToVector2());

                foreach (Win2DRenderNode renderNode in renderer.RenderNodes)
                {
                    if (renderNode.HitTest(position))
                    {
                        textEditor.BeginEdit(renderNode);

                        renderer.Invalidate();
                        
                        break;
                    }
                }
            });
        }

        protected override void OnTapped(TappedRoutedEventArgs e)
        {
            WithRenderer(renderer =>
            {
                if (textEditor != null)
                {
                    Vector2 position = renderer.GetMindmapPosition(e.GetPosition(this).ToVector2());

                    Win2DRenderNode handledNode;

                    if (!renderer.HandleClick(position, out handledNode) || handledNode != textEditor.EditingNode)
                    {
                        textEditor.EndEdit();

                        renderer.Invalidate();
                    }
                }
            });
        }

        private void WithRenderer(Action<Win2DRenderer> action)
        {
            if (renderer != null)
            {
                action(renderer);
            }
        }
    }
}
