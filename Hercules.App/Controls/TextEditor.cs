// ==========================================================================
// TextEditor.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using GP.Utils.UI.Controls;
using Hercules.Model;
using Hercules.Win2D.Rendering;

namespace Hercules.App.Controls
{
    public sealed class TextEditor : AdvancedTextBox
    {
        private readonly TranslateTransform renderTransform = new TranslateTransform();
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

        public void BeginEdit(Win2DRenderNode renderNode)
        {
            if (renderNode == null)
            {
                return;
            }

            if (editingNode != renderNode)
            {
                EndEdit(false);

                editingNode = renderNode;
            }

            UpdateText();

            Show();
        }

        public void EndEdit(bool invokeEvent)
        {
            if (editingNode == null)
            {
                return;
            }

            try
            {
                editingNode.Node.ChangeTextTransactional(Text);
            }
            finally
            {
                CancelEdit(invokeEvent);
            }
        }

        public void CancelEdit(bool invokeEvent)
        {
            if (editingNode == null)
            {
                return;
            }

            Hide();

            editingNode = null;

            if (invokeEvent)
            {
                OnEditingEnded();
            }
        }

        public void Transform()
        {
            if (editingNode == null)
            {
                return;
            }

            var position = editingNode.TextRenderer.RenderPosition;

            renderTransform.X = position.X - Padding.Left - BorderThickness.Left;
            renderTransform.Y = position.Y - Padding.Top  - BorderThickness.Top;

            var renderSize = editingNode.TextRenderer.RenderSize;

            MinWidth =
                renderSize.X +
                Padding.Left +
                Padding.Right +
                BorderThickness.Left +
                BorderThickness.Right;

            MinHeight = renderSize.Y + Padding.Top + Padding.Bottom + BorderThickness.Top + BorderThickness.Bottom;
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
