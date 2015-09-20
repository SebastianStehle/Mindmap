// ==========================================================================
// Mindmap.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System;
using System.Numerics;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Hercules.Model;
using Hercules.Model.Layouting;
using Hercules.Model.Rendering.Win2D;
using Hercules.Model.Utils;
using Microsoft.Graphics.Canvas.UI.Xaml;

namespace Hercules.App.Controls
{
    [TemplatePart(Name = CanvasPart, Type = typeof(CanvasControl))]
    [TemplatePart(Name = TextBoxPart, Type = typeof(TextEditor))]
    [TemplatePart(Name = ScrollViewerPart, Type = typeof(ScrollViewer))]
    public sealed class Mindmap : Control
    {
        private const string CanvasPart = "PART_Canvas";
        private const string TextBoxPart = "PART_TextBox";
        private const string ScrollViewerPart = "PART_ScrollViewer";
        private IRendererFactory lastRendererFactory;
        private ScrollViewer scrollViewer;
        private Win2DRenderer renderer;
        private CanvasControlWrapper canvasControl;
        private TextEditor textEditor;
        private ScrollViewerView lastView;

        public Win2DRenderNode TextEditingNode
        {
            get { return textEditor.EditingNode; }
        }

        public Win2DRenderer Renderer
        {
            get { return renderer; }
        }

        public static readonly DependencyProperty LayoutProperty =
            DependencyProperty.Register("Layout", typeof(ILayout), typeof(Mindmap), new PropertyMetadata(null, OnLayoutChanged));
        public ILayout Layout
        {
            get { return (ILayout)GetValue(LayoutProperty); }
            set { SetValue(LayoutProperty, value); }
        }

        private static void OnLayoutChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            var owner = o as Mindmap;

            owner?.InitializeLayout();
        }

        public static readonly DependencyProperty RendererFactoryProperty =
            DependencyProperty.Register("RendererFactory", typeof(IRendererFactory), typeof(Mindmap), new PropertyMetadata(null, OnRendererChanged));
        public IRendererFactory RendererFactory
        {
            get { return (IRendererFactory)GetValue(RendererFactoryProperty); }
            set { SetValue(RendererFactoryProperty, value); }
        }

        private static void OnRendererChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            var owner = o as Mindmap;

            owner?.InitializeRenderer();
        }

        public static readonly DependencyProperty DocumentProperty =
            DependencyProperty.Register("Document", typeof(Document), typeof(Mindmap), new PropertyMetadata(null, OnDocumentChanged));
        public Document Document
        {
            get { return (Document)GetValue(DocumentProperty); }
            set { SetValue(DocumentProperty, value); }
        }

        public static void OnDocumentChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            var owner = o as Mindmap;

            owner?.InitializeRenderer();
        }

        public Mindmap()
        {
            DefaultStyleKey = typeof(Mindmap);
        }

        protected override void OnApplyTemplate()
        {
            BindCanvasControl();
            BindTextEditor();
            BindScrollViewer();

            InitializeRenderer();
        }

        private void BindTextEditor()
        {
            textEditor = GetTemplateChild(TextBoxPart) as TextEditor;
        }

        private void BindScrollViewer()
        {
            scrollViewer = GetTemplateChild(ScrollViewerPart) as ScrollViewer;

            if (scrollViewer != null)
            {
                scrollViewer.ViewChanging += ScrollViewer_ViewChanging;

                scrollViewer.ZoomSnapPoints.Add(0.5f);
                scrollViewer.ZoomSnapPoints.Add(1.0f);
                scrollViewer.ZoomSnapPoints.Add(2.0f);
            }
        }

        private void BindCanvasControl()
        {
            CanvasControl control = GetTemplateChild(CanvasPart) as CanvasControl;

            if (control != null)
            {
                canvasControl = new CanvasControlWrapper(control);

                canvasControl.Draw += CanvasControl_Draw;
            }
        }

        private void ScrollViewer_ViewChanging(object sender, ScrollViewerViewChangingEventArgs e)
        {
            WithRenderer(r =>
            {
                if (Document == null)
                {
                    return;
                }

                ScrollViewerView view = e.FinalView;

                if (lastView == null || (view.ZoomFactor != lastView.ZoomFactor || view.HorizontalOffset != lastView.HorizontalOffset || view.VerticalOffset != lastView.VerticalOffset))
                {
                    lastView = view;

                    Transform(r, view);
                }
            });
        }

        private void Transform(Win2DRenderer rendererToTransform, ScrollViewerView view)
        {
            double zoomFactor = view.ZoomFactor;

            double xOffset = view.HorizontalOffset;
            double yOffset = view.VerticalOffset;

            double inverseZoom = 1.0 / zoomFactor;

            double scaledContentW = Document.Size.X * zoomFactor;
            double scaledContentH = Document.Size.Y * zoomFactor;

            double translateX;
            double translateY;

            if (scaledContentW < scrollViewer.ViewportWidth)
            {
                translateX = ((scrollViewer.ViewportWidth * inverseZoom) - Document.Size.X) * 0.5;
            }
            else
            {
                translateX = -inverseZoom * xOffset;
            }

            if (scaledContentH < scrollViewer.ViewportHeight)
            {
                translateY = ((scrollViewer.ViewportHeight * inverseZoom) - Document.Size.Y) * 0.5;
            }
            else
            {
                translateY = -inverseZoom * yOffset;
            }

            double visibleX = inverseZoom * xOffset;
            double visibleY = inverseZoom * yOffset;

            double visibleW = Math.Min(Document.Size.X, inverseZoom * scrollViewer.ViewportWidth);
            double visibleH = Math.Min(Document.Size.Y, inverseZoom * scrollViewer.ViewportHeight);

            Rect2 visibleRect = new Rect2((float)visibleX, (float)visibleY, (float)visibleW, (float)visibleH);

            rendererToTransform.Transform(new Vector2((float)translateX, (float)translateY), (float)zoomFactor, visibleRect);
            rendererToTransform.InvalidateWithoutLayout();
        }

        private void CanvasControl_Draw(object sender, CanvasDrawEventArgs e)
        {
            WithRenderer(r => textEditor.UpdateTransform());
        }

        private void InitializeRenderer()
        {
            if (textEditor != null)
            {
                textEditor.CancelEdit();
            }

            if (renderer != null)
            {
                renderer.Dispose();
                renderer = null;
            }

            if (canvasControl != null && Document != null && RendererFactory != null)
            {
                if (renderer == null || renderer.Document != Document || renderer.Canvas != canvasControl || RendererFactory != lastRendererFactory)
                {
                    renderer = RendererFactory.CreateRenderer(Document, canvasControl);

                    lastRendererFactory = RendererFactory;
                }
            }

            InitializeLayout();

            if (renderer != null && lastView != null)
            {
                Transform(renderer, lastView);
            }
            else if (canvasControl != null)
            {
                canvasControl.Invalidate();
            }
        }

        private void InitializeLayout()
        {
            if (renderer != null && Layout != null)
            {
                renderer.Layout = Layout;
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

        public void EditText()
        {
            WithRenderer(r =>
            {
                if (Document?.SelectedNode != null)
                {
                    Win2DRenderNode renderNode = (Win2DRenderNode)r.FindRenderNode(Document.SelectedNode);

                    textEditor.BeginEdit(renderNode);

                    r.Invalidate();
                }
            });
        }

        protected override void OnDoubleTapped(DoubleTappedRoutedEventArgs e)
        {
            WithRenderer(r =>
            {
                Vector2 position = r.GetMindmapPosition(e.GetPosition(this).ToVector2());

                foreach (Win2DRenderNode renderNode in r.RenderNodes)
                {
                    if (renderNode.HitTest(position))
                    {
                        textEditor.BeginEdit(renderNode);

                        r.Invalidate();
                        
                        break;
                    }
                }
            });
        }

        protected override void OnTapped(TappedRoutedEventArgs e)
        {
            WithRenderer(r =>
            {
                if (textEditor != null)
                {
                    Vector2 position = r.GetMindmapPosition(e.GetPosition(this).ToVector2());

                    Win2DRenderNode handledNode;

                    if (!r.HandleClick(position, out handledNode) || handledNode != textEditor.EditingNode)
                    {
                        textEditor.EndEdit();

                        r.Invalidate();
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
