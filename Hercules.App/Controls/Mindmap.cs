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
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;
using GP.Utils;
using GP.Utils.UI;
using GP.Utils.UI.Controls;
using Hercules.App.Modules.Editor.Views;
using Hercules.Model;
using Hercules.Win2D.Rendering;

// ReSharper disable InvertIf
// ReSharper disable LoopCanBeConvertedToQuery
// ReSharper disable SuggestBaseTypeForParameter
// ReSharper disable LoopCanBePartlyConvertedToQuery

namespace Hercules.App.Controls
{
    [TemplatePart(Name = PartCanvas, Type = typeof(CanvasVirtualWrapper))]
    [TemplatePart(Name = PartTextBox, Type = typeof(TextEditor))]
    [TemplatePart(Name = PartScrollViewer, Type = typeof(ScrollViewer))]
    public sealed class Mindmap : Control
    {
        private const string PartCanvas = "PART_Canvas";
        private const string PartTextBox = "PART_TextBox";
        private const string PartScrollViewer = "PART_ScrollViewer";
        private IWin2DRendererProvider lastWin2DRendererProvider;
        private CanvasVirtualWrapper canvasControl;
        private Win2DRenderer renderer;
        private ScrollViewer scrollViewer;
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

        public static readonly DependencyProperty NotesFlyoutStyleProperty =
            DependencyPropertyManager.Register<Mindmap, Style>(nameof(NotesFlyoutStyle), null);
        public Style NotesFlyoutStyle
        {
            get { return (Style)GetValue(NotesFlyoutStyleProperty); }
            set { SetValue(NotesFlyoutStyleProperty, value); }
        }

        public static readonly DependencyProperty DocumentProperty =
            DependencyPropertyManager.Register<Mindmap, Document>(nameof(Document), null, (d, e) => d.OnDocumentChanged(e));
        public Document Document
        {
            get { return (Document)GetValue(DocumentProperty); }
            set { SetValue(DocumentProperty, value); }
        }

        public static readonly DependencyProperty RendererProviderProperty =
            DependencyPropertyManager.Register<Mindmap, IWin2DRendererProvider>(nameof(RendererProvider), null, (d, e) => d.InitializeRenderer());
        public IWin2DRendererProvider RendererProvider
        {
            get { return (IWin2DRendererProvider)GetValue(RendererProviderProperty); }
            set { SetValue(RendererProviderProperty, value); }
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

        private void OnDocumentChanged(DependencyPropertyChangedEventArgs e)
        {
            Document oldDocument = e.OldValue as Document;

            if (oldDocument != null)
            {
                oldDocument.StateChanged -= Document_StateChanged;
            }

            Document newDocument = e.NewValue as Document;

            if (newDocument != null)
            {
                newDocument.StateChanged += Document_StateChanged;
            }

            InitializeRenderer();
        }

        private void Document_StateChanged(object sender, StateChangedEventArgs e)
        {
            IUndoRedoManager undoRedoManager = (IUndoRedoManager)sender;

            ToggleNotesCommand command = undoRedoManager.LastCommand<ToggleNotesCommand>(n => n.Node.IsNotesEnabled);

            if (command != null)
            {
                ShowNotes(command.Node);
            }
        }

        private void BindTextEditor()
        {
            textEditor = GetTemplateChild(PartTextBox) as TextEditor;

            if (textEditor != null)
            {
                textEditor.EditingEnded += TextEditor_EditingEnded;
            }
        }

        private void BindScrollViewer()
        {
            scrollViewer = GetTemplateChild(PartScrollViewer) as ScrollViewer;

            if (scrollViewer != null)
            {
                scrollViewer.ViewChanged += ScrollViewer_ViewChanged;
            }
        }

        private void BindCanvasControl()
        {
            canvasControl = GetTemplateChild(PartCanvas) as CanvasVirtualWrapper;

            if (canvasControl != null)
            {
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

            if (renderer != null)
            {
                UpdateScale();
            }
            if (canvasControl != null)
            {
                canvasControl.Invalidate();
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
                Vector2 position = e.GetCurrentPoint(canvasControl).Position.ToVector2();

                HitResult result = r.HitTest(position);

                if (result?.RenderNode == textEditor.EditingNode)
                {
                    return;
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
                Vector2 position = e.GetPosition(canvasControl).ToVector2();

                HitResult hitResult = r.Scene.HitTest(position);

                if (hitResult != null)
                {
                    textEditor.BeginEdit(hitResult.RenderNode);
                    textEditor.Transform();
                }
            });
        }

        protected override void OnTapped(TappedRoutedEventArgs e)
        {
            WithRenderer(r =>
            {
                if (textEditor != null)
                {
                    Vector2 position = e.GetPosition(canvasControl).ToVector2();

                    HitResult hitResult = r.Scene.HitTest(position);

                    if (hitResult == null || hitResult.RenderNode != textEditor.EditingNode)
                    {
                        textEditor.EndEdit(true);
                    }

                    if (hitResult != null)
                    {
                        if (hitResult.Target == HitTarget.Node)
                        {
                            hitResult.RenderNode.Node.Select();
                        }
                        else if (hitResult.Target == HitTarget.ExpandButton)
                        {
                            hitResult.RenderNode.Node.ToggleCollapseTransactional();
                        }
                        else if (hitResult.Target == HitTarget.CheckBox)
                        {
                            hitResult.RenderNode.Node.ToggleCheckedTransactional();
                        }
                        else if (hitResult.Target == HitTarget.NotesButton)
                        {
                            ShowNotes(hitResult.RenderNode.Node);
                        }
                    }
                }
            });
        }

        private void ShowNotes(NodeBase node)
        {
            Flyout flyout = new Flyout { FlyoutPresenterStyle = NotesFlyoutStyle, Placement = FlyoutPlacementMode.Full };

            NotesEditor editor = new NotesEditor(flyout, node);

            flyout.Content = editor;
            flyout.ShowAt(this);
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
