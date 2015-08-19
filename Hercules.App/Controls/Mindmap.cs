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
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Hercules.Model.Rendering.Win2D;
using GP.Windows.UI.Controls;

// ReSharper disable UnusedParameter.Local

namespace Hercules.App.Controls
{
    [TemplatePart(Name = CanvasPart, Type = typeof(ZoomableCanvasControl))]
    [TemplatePart(Name = TextBoxPart, Type = typeof(TextEditor))]
    public sealed class Mindmap : Control
    {
        private const string TextBoxPart = "PART_TextBox";
        private const string CanvasPart = "PART_Canvas";
        private IRendererFactory lastRendererFactory;
        private Win2DRenderer renderer;
        private ZoomableCanvasControl canvasControl;
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
            DependencyProperty.Register("RendererFactory", typeof(IRendererFactory), typeof(Mindmap), new PropertyMetadata(null, OnDocumentRendererChanged));
        public IRendererFactory RendererFactory
        {
            get { return (IRendererFactory)GetValue(RendererFactoryProperty); }
            set { SetValue(RendererFactoryProperty, value); }
        }

        public static readonly DependencyProperty DocumentProperty =
            DependencyProperty.Register("Document", typeof(Document), typeof(Mindmap), new PropertyMetadata(null, OnDocumentRendererChanged));
        public Document Document
        {
            get { return (Document)GetValue(DocumentProperty); }
            set { SetValue(DocumentProperty, value); }
        }

        private static void OnDocumentRendererChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
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
            canvasControl = GetTemplateChild(CanvasPart) as ZoomableCanvasControl;

            textEditor = GetTemplateChild(TextBoxPart) as TextEditor;

            InitializeRenderer();
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

        protected override void OnDoubleTapped(DoubleTappedRoutedEventArgs e)
        {
            WithRenderer(r =>
            {
                Vector2 position = e.GetPosition(this).ToVector2();

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
                    Vector2 position = e.GetPosition(this).ToVector2();

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
