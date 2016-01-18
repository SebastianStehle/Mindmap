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
using Hercules.Win2D.Rendering;
using Microsoft.Graphics.Canvas.UI.Xaml;

// ReSharper disable InvertIf
// ReSharper disable LoopCanBeConvertedToQuery
// ReSharper disable SuggestBaseTypeForParameter
// ReSharper disable LoopCanBePartlyConvertedToQuery

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
        private IWin2DRendererProvider lastWin2DRendererProvider;
        private ScrollViewer scrollViewer;
        private Win2DRenderer renderer;
        private CanvasControlWrapper canvasControl;
        private TextEditor textEditor;

        public Win2DRenderNode TextEditingNode
        {
            get { return textEditor.EditingNode; }
        }

        public Win2DRenderer Renderer
        {
            get { return renderer; }
        }

        public Win2DScene Scene
        {
            get { return renderer.Scene; }
        }

        public static readonly DependencyProperty DocumentProperty =
            DependencyProperty.Register(nameof(Document), typeof(Document), typeof(Mindmap), new PropertyMetadata(null, OnDocumentChanged));
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

        public static readonly DependencyProperty LayoutProperty =
            DependencyProperty.Register(nameof(Layout), typeof(ILayout), typeof(Mindmap), new PropertyMetadata(null, OnLayoutChanged));
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

        public static readonly DependencyProperty RendererProviderProperty =
            DependencyProperty.Register(nameof(RendererProvider), typeof(IWin2DRendererProvider), typeof(Mindmap), new PropertyMetadata(null, OnRendererChanged));
        public IWin2DRendererProvider RendererProvider
        {
            get { return (IWin2DRendererProvider)GetValue(RendererProviderProperty); }
            set { SetValue(RendererProviderProperty, value); }
        }

        private static void OnRendererChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
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

            if (textEditor != null)
            {
                textEditor.EditingEnded += TextEditor_EditingEnded;
            }
        }

        private void BindScrollViewer()
        {
            scrollViewer = GetTemplateChild(ScrollViewerPart) as ScrollViewer;

            if (scrollViewer != null)
            {
                scrollViewer.ViewChanged += ScrollViewer_ViewChanged;
            }
        }

        private void BindCanvasControl()
        {
            CanvasVirtualControl control = GetTemplateChild(CanvasPart) as CanvasVirtualControl;

            if (control != null)
            {
                canvasControl = new CanvasControlWrapper(control);

                canvasControl.AfterDraw += CanvasControl_AfterDraw;
            }
        }

        private void ScrollViewer_ViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
        {
            WithRenderer(r =>
            {
                if (!e.IsIntermediate)
                {
                    UpdateScale();
                }
            });
        }

        private void UpdateScale()
        {
            if (canvasControl != null)
            {
                float dpiScale = scrollViewer.ZoomFactor;

                float dpiRatio = canvasControl.DpiScale / dpiScale;

                if (dpiRatio <= 0.8 || dpiRatio >= 1.25)
                {
                    canvasControl.DpiScale = dpiScale;
                }
            }
        }

        private void CanvasControl_AfterDraw(object sender, EventArgs e)
        {
            WithRenderer(r => textEditor.Transform());
        }

        private void InitializeRenderer()
        {
            if (textEditor != null)
            {
                textEditor.CancelEdit(true);
            }

            if (renderer != null)
            {
                renderer.Dispose();
                renderer = null;
            }

            if (canvasControl != null && Document != null && RendererProvider != null)
            {
                if (renderer == null || renderer.Document != Document || renderer.Canvas != canvasControl || RendererProvider != lastWin2DRendererProvider)
                {
                    renderer = RendererProvider.CreateRenderer(Document, canvasControl);

                    lastWin2DRendererProvider = RendererProvider;
                }
            }

            InitializeLayout();

            if (renderer != null)
            {
                UpdateScale();
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

        private void TextEditor_EditingEnded(object sender, EventArgs e)
        {
            Focus(FocusState.Programmatic);
        }

        protected override void OnPointerPressed(PointerRoutedEventArgs e)
        {
            FocusWhenNotTextEditing(e);
        }

        protected override void OnPointerReleased(PointerRoutedEventArgs e)
        {
            FocusWhenNotTextEditing(e);
        }

        private void FocusWhenNotTextEditing(PointerRoutedEventArgs e)
        {
            WithRenderer(r =>
            {
                Vector2 position = e.GetCurrentPoint(canvasControl.Inner).Position.ToVector2();

                foreach (Win2DRenderNode renderNode in r.Scene.DiagramNodes)
                {
                    if (renderNode.HitTest(position) && textEditor.EditingNode == renderNode)
                    {
                        return;
                    }
                }

                Focus(FocusState.Programmatic);

                e.Handled = true;
            });
        }

        public void EditText()
        {
            WithRenderer(r =>
            {
                if (Document?.SelectedNode != null)
                {
                    Win2DRenderNode renderNode = (Win2DRenderNode)r.Scene.FindRenderNode(Document.SelectedNode);

                    textEditor.BeginEdit(renderNode);
                    textEditor.Transform();
                }
            });
        }

        protected override void OnDoubleTapped(DoubleTappedRoutedEventArgs e)
        {
            WithRenderer(r =>
            {
                Vector2 position = e.GetPosition(canvasControl.Inner).ToVector2();

                foreach (Win2DRenderNode renderNode in r.Scene.DiagramNodes)
                {
                    if (renderNode.HitTest(position))
                    {
                        textEditor.BeginEdit(renderNode);
                        textEditor.Transform();
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
                    Vector2 position = e.GetPosition(canvasControl.Inner).ToVector2();

                    Win2DRenderNode handledNode;

                    if (!r.HandleClick(position, out handledNode) || handledNode != textEditor.EditingNode)
                    {
                        textEditor.EndEdit(true);
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
