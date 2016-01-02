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
using Windows.UI.Xaml;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using GP.Windows.UI.Controls;
using Hercules.Model;
using Hercules.Win2D.Rendering;

namespace Hercules.App.Controls
{
    public sealed class TextEditor : AdvancedTextBox
    {
        private readonly CompositeTransform renderTransform = new CompositeTransform();
        private Win2DRenderNode editingNode;

        public Win2DRenderNode EditingNode
        {
            get { return editingNode; }
        }

        public event EventHandler EditingEnded;

        public TextEditor()
        {
            RenderTransform = renderTransform;
        }

        protected override void OnLostFocus(RoutedEventArgs e)
        {
            EndEdit(true);

            base.OnLostFocus(e);
        }

        protected override void OnEnter(KeyRoutedEventArgs e)
        {
            EndEdit(true);
        }

        protected override void OnReset(KeyRoutedEventArgs e)
        {
            CancelEdit(true);
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
                    EndEdit(false);

                    editingNode = renderNode;
                }

                UpdateTransform();
                UpdateText();

                Show();
            }
        }

        public void EndEdit(bool invokeEvent)
        {
            if (editingNode != null)
            {
                try
                {
                    editingNode.Node.ChangeTextTransactional(Text);
                }
                finally
                {
                    CancelEdit(invokeEvent);
                }
            }
        }

        public void CancelEdit(bool invokeEvent)
        {
            if (editingNode != null)
            {
                Hide();

                editingNode = null;

                if (invokeEvent)
                {
                    OnEditingEnded();
                }
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
            if (editingNode != null)
            {
                Vector2 renderSize = editingNode.TextRenderer.RenderSize;

                Size actualSize = base.MeasureOverride(availableSize);

                double minSizeX = Math.Max(MinWidth, renderSize.X + Padding.Left + Padding.Right + BorderThickness.Left + BorderThickness.Right);

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

            return new Size(0, 0);
        }

        private void UpdateText()
        {
            Text = editingNode.Node.Text ?? string.Empty;
        }

        private void OnEditingEnded()
        {
            EditingEnded?.Invoke(this, EventArgs.Empty);
        }
    }
}
