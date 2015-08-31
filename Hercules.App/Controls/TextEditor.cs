// ==========================================================================
// TextEditor.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System;
using System.Numerics;
using Windows.Foundation;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Hercules.App.Components.Implementations;
using Hercules.Model;
using Hercules.Model.Rendering.Win2D;
using Windows.UI.Xaml.Input;

namespace Hercules.App.Controls
{
    public sealed class TextEditor : TextBox
    {
        private readonly CompositeTransform renderTransform = new CompositeTransform();
        private Win2DRenderNode editingNode;

        public Win2DRenderNode EditingNode
        {
            get { return editingNode; }
        }

        private bool IsEditing
        {
            get { return editingNode != null; }
        }

        public TextEditor()
        {
            RenderTransform = renderTransform;
        }

        protected override void OnLostFocus(RoutedEventArgs e)
        {
            CancelEdit();

            base.OnLostFocus(e);
        }

        protected override void OnGotFocus(RoutedEventArgs e)
        {
            if (IsEditing && !string.IsNullOrWhiteSpace(Text))
            {
                Select(0, Text.Length);
            }

            base.OnGotFocus(e);
        }

        protected override void OnKeyDown(KeyRoutedEventArgs e)
        {
            if (IsEditing)
            {
                if (e.Key == VirtualKey.Enter)
                {
                    CoreVirtualKeyStates shiftKeyState = Window.Current.CoreWindow.GetKeyState(VirtualKey.Shift);

                    if ((shiftKeyState & CoreVirtualKeyStates.Down) != CoreVirtualKeyStates.Down)
                    {
                        EndEdit();

                        e.Handled = true;
                    }
                }
                else if (e.Key == VirtualKey.Escape)
                {
                    CancelEdit();

                    e.Handled = true;
                }
            }

            base.OnKeyDown(e);
        }

        public void UpdateTransform()
        {
            InvalidateMeasure();
            InvalidateArrange();
        }

        public void BeginEdit(Win2DRenderNode renderNode)
        {
            if (renderNode != null)
            {
                if (editingNode != renderNode)
                {
                    EndEdit();

                    BindToNode(renderNode);

                    UpdateTransform();
                    UpdateStyle();
                    UpdateText();

                    Show();
                }

                Focus(FocusState.Pointer);
            }
        }

        public void EndEdit()
        {
            if (IsEditing)
            {
                try
                {
                    string transactionName = ResourceManager.GetString("ChangeTextTransactionName");

                    editingNode.Node.Document.MakeTransaction(transactionName, commands =>
                    {
                        commands.Apply(new ChangeTextCommand(editingNode.Node, Text, true));
                    });
                }
                finally
                {
                    CancelEdit();
                }
            }
        }

        public void CancelEdit()
        {
            if (IsEditing)
            {
                Hide();

                UnbindFromNode();
            }
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            if (editingNode != null)
            {
                Vector2 position = editingNode.Renderer.GetOverlayPosition(editingNode.TextRenderer.RenderPosition);

                renderTransform.TranslateX = position.X - Padding.Left - BorderThickness.Left;
                renderTransform.TranslateY = position.Y - Padding.Top - BorderThickness.Top;

                float zoom = editingNode.Renderer.ZoomFactor;

                renderTransform.ScaleX = zoom;
                renderTransform.ScaleY = zoom;
            }

            return base.ArrangeOverride(finalSize);
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            if (IsEditing)
            {
                Vector2 renderSize = editingNode.TextRenderer.RenderSize;
                
                Size actualSize = base.MeasureOverride(availableSize);

                double minSizeX = renderSize.X + Padding.Left + Padding.Right + BorderThickness.Left + BorderThickness.Right;

                if (actualSize.Width < minSizeX)
                {
                    actualSize.Width = minSizeX;
                }

                double minSizeY = renderSize.Y + Padding.Top + Padding.Bottom + BorderThickness.Top + BorderThickness.Bottom;

                if (actualSize.Height < minSizeY)
                {
                    actualSize.Height = minSizeY;
                }

                return actualSize;
            }
            else
            {
                return new Size(0, 0);
            }
        }

        private void BindToNode(Win2DRenderNode renderNode)
        {
            editingNode = renderNode;
        }

        private void UnbindFromNode()
        {
            editingNode = null;
        }

        private void UpdateText()
        {
            Text = editingNode.Node.Text ?? string.Empty;
        }

        private void UpdateStyle()
        {
            FontSize = editingNode.TextRenderer.FontSize;
        }

        private void Show()
        {
            Visibility = Visibility.Visible;
        }

        private void Hide()
        {
            Visibility = Visibility.Collapsed;
        }
    }
}
