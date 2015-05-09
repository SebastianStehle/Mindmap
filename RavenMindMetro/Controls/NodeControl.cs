// ==========================================================================
// NodeControl.cs
// RavenMind Application
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using RavenMind.Model;
using SE.Metro.UI;
using SE.Metro.UI.Controls;
using System.ComponentModel;
using Windows.Foundation;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;

namespace RavenMind.Controls
{
    [TemplatePart(Name = PartTextBox, Type = typeof(TextBox))]
    public class NodeControl : LoadableControl
    {
        private const string PartTextBox = "TextBox";
        private TextBox textBox;
        private bool isCancelled;

        public static readonly DependencyProperty IsTextEditingProperty =
            DependencyProperty.Register("IsTextEditing", typeof(bool), typeof(NodeControl), new PropertyMetadata(false));
        public bool IsTextEditing
        {
            get { return (bool)GetValue(IsTextEditingProperty); }
            set { SetValue(IsTextEditingProperty, value); }
        }

        public static readonly DependencyProperty AssociatedNodeProperty =
            DependencyProperty.Register("AssociatedNode", typeof(NodeBase), typeof(NodeControl), new PropertyMetadata(null));
        public NodeBase AssociatedNode
        {
            get { return (NodeBase)GetValue(AssociatedNodeProperty); }
            set { SetValue(AssociatedNodeProperty, value); }
        }

        public static readonly DependencyProperty AnchorProperty =
            DependencyProperty.Register("Anchor", typeof(AnchorPoint), typeof(NodeControl), new PropertyMetadata(AnchorPoint.Center, new PropertyChangedCallback(OnPositioningChanged)));
        public AnchorPoint Anchor
        {
            get { return (AnchorPoint)GetValue(AnchorProperty); }
            set { SetValue(AnchorProperty, value); }
        }

        public static readonly DependencyProperty AnchorPositionProperty =
            DependencyProperty.Register("AnchorPosition", typeof(Point), typeof(NodeControl), new PropertyMetadata(new Point(0, 0)));
        public Point AnchorPosition
        {
            get { return (Point)GetValue(AnchorPositionProperty); }
            set { SetValue(AnchorPositionProperty, value); }
        }

        public static readonly DependencyProperty PositionProperty =
            DependencyProperty.Register("Position", typeof(Point), typeof(NodeControl), new PropertyMetadata(new Point(), new PropertyChangedCallback(OnPositioningChanged)));
        public Point Position
        {
            get { return (Point)GetValue(PositionProperty); }
            set { SetValue(PositionProperty, value); }
        }

        private static void OnPositioningChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            var owner = o as NodeControl;
            if (owner != null)
            {
                owner.OnPositioningChanged();
            }
        }

        private void OnPositioningChanged()
        {
            MindmapPanel panel = VisualTreeHelper.GetParent(this) as MindmapPanel;

            if (panel != null)
            {
                panel.InvalidateArrange();
            }
        }

        public NodeControl()
        {
            DefaultStyleKey = typeof(NodeControl);
        }

        public void Attach(NodeBase node)
        {
            AssociatedNode = node;

            if (AssociatedNode != null)
            {
                AssociatedNode.PropertyChanged += AssociatedNode_PropertyChanged;

                DataContext = node;

                UpdateSelection();
            }
        }

        public void Detach()
        {
            if (AssociatedNode != null)
            {
                AssociatedNode.PropertyChanged -= AssociatedNode_PropertyChanged;
                AssociatedNode = null;

                DataContext = null;
            }
        }

        protected override void OnApplyTemplate()
        {
            textBox = GetTemplateChild(PartTextBox) as TextBox;

            if (textBox != null)
            {
                textBox.LostFocus += textBox_LostFocus;
                textBox.GotFocus += textBox_GotFocus;
                textBox.KeyDown += textBox_KeyDown;
            }
        }

        public void InvalidateLayout()
        {
            MindmapPanel panel = VisualTreeHelper.GetParent(this) as MindmapPanel;

            if (panel != null)
            {
                panel.InvalidateLayout();
            }
        }

        private void AssociatedNode_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsSelected")
            {
                UpdateSelection();
            }
        }

        private void UpdateSelection()
        {
            if (AssociatedNode.IsSelected)
            {
                ManipulationMode = ManipulationModes.TranslateX | ManipulationModes.TranslateY;

                Focus(FocusState.Pointer);
            }
            else
            {
                ManipulationMode = ManipulationModes.System;
            }
        }

        protected override void OnKeyDown(KeyRoutedEventArgs e)
        {
            if (AssociatedNode.IsSelected && e.Key.IsLetterOrNumber())
            {
                textBox.Focus(FocusState.Keyboard);
            }
        }

        private void textBox_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == VirtualKey.Enter || e.Key == VirtualKey.Escape)
            {
                if (e.Key == VirtualKey.Escape)
                {
                    isCancelled = true;
                }

                Focus(FocusState.Pointer);

                e.Handled = true;
            }
        }

        protected override void OnDoubleTapped(DoubleTappedRoutedEventArgs e)
        {
            AssociatedNode.Select();

            if (textBox != null)
            {
                textBox.Focus(FocusState.Pointer);
            }
        }

        protected override void OnPointerPressed(PointerRoutedEventArgs e)
        {
            AssociatedNode.Select();
        }

        private void textBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(textBox.Text))
            {
                textBox.Select(textBox.Text.Length, 1);
            }

            this.MakePanelAnimated(false);

            IsTextEditing = true;
        }

        private void textBox_LostFocus(object sender, RoutedEventArgs e)
        {
            this.MakePanelAnimated(true);

            if (AssociatedNode != null)
            {
                if (!isCancelled)
                {
                    AssociatedNode.Document.MakeTransaction("Change Text", c =>
                    {
                        c.Apply(new ChangeTextCommand(AssociatedNode, textBox.Text));
                    });
                }
                else
                {
                    isCancelled = false;
                }
            }

            IsTextEditing = false;
        }
    }
}
