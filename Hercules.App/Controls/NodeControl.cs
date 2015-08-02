// ==========================================================================
// NodeControl.cs
// Mindmap Application
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using Hercules.Model;
using GP.Windows.UI;
using GP.Windows.UI.Controls;
using System.ComponentModel;
using Windows.Foundation;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace Hercules.App.Controls
{
    [TemplatePart(Name = PartTextBox, Type = typeof(TextBox))]
    [TemplatePart(Name = PartToggleButton, Type = typeof(CheckBox))]
    public class NodeControl : LoadableControl
    {
        private const string PartTextBox = "TextBox";
        private const string PartToggleButton = "ToggleButton";
        private NodeBase associatedNode;
        private CheckBox toggleButton;
        private TextBox textBox;
        private bool isCancelled;

        public static readonly DependencyProperty ThemeColorProperty =
            DependencyProperty.Register("ThemeColor", typeof(ThemeColor), typeof(NodeControl), new PropertyMetadata(ThemeColor.White));
        public ThemeColor ThemeColor
        {
            get { return (ThemeColor)GetValue(ThemeColorProperty); }
            set { SetValue(ThemeColorProperty, value); }
        }

        public static readonly DependencyProperty IsTextEditingProperty =
            DependencyProperty.Register("IsTextEditing", typeof(bool), typeof(NodeControl), new PropertyMetadata(false));
        public bool IsTextEditing
        {
            get { return (bool)GetValue(IsTextEditingProperty); }
            set { SetValue(IsTextEditingProperty, value); }
        }

        public static readonly DependencyProperty AnchorPositionProperty =
            DependencyProperty.Register("AnchorPosition", typeof(Point), typeof(NodeControl), new PropertyMetadata(new Point(0, 0)));
        public Point AnchorPosition
        {
            get { return (Point)GetValue(AnchorPositionProperty); }
            set { SetValue(AnchorPositionProperty, value); }
        }

        public NodeBase AssociatedNode
        {
            get
            {
                return associatedNode;
            }
        }

        public NodeControl()
        {
            DefaultStyleKey = typeof(NodeControl);
        }

        public void Attach(NodeBase node)
        {
            associatedNode = node;

            if (associatedNode != null)
            {
                associatedNode.PropertyChanged += AssociatedNode_PropertyChanged;

                DataContext = node;

                UpdateSelection();
            }
        }

        public void Detach()
        {
            if (associatedNode != null)
            {
                associatedNode.PropertyChanged -= AssociatedNode_PropertyChanged;
                associatedNode = null;

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

            toggleButton = GetTemplateChild(PartToggleButton) as CheckBox;

            if (toggleButton != null)
            {
                toggleButton.Click += toggleButton_Click;
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
            if (associatedNode.IsSelected)
            {
                ManipulationMode = ManipulationModes.TranslateX | ManipulationModes.TranslateY;

                Focus(FocusState.Pointer);
            }
            else
            {
                ManipulationMode = ManipulationModes.System;
            }
        }

        protected override void OnDoubleTapped(DoubleTappedRoutedEventArgs e)
        {
            associatedNode.Select();

            if (textBox != null)
            {
                textBox.Focus(FocusState.Pointer);
            }
        }

        protected override void OnKeyDown(KeyRoutedEventArgs e)
        {
            if (associatedNode.IsSelected && e.Key.IsLetterOrNumber())
            {
                textBox.Focus(FocusState.Keyboard);
            }
        }

        protected override void OnPointerPressed(PointerRoutedEventArgs e)
        {
            associatedNode.Select();
        }

        private void toggleButton_Click(object sender, RoutedEventArgs e)
        {
            associatedNode.Document.MakeTransaction("Toggle", c =>
            {
                c.Apply(new ToggleCollapseCommand(associatedNode));
            });
        }

        private void textBox_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == VirtualKey.Enter || e.Key == VirtualKey.Escape)
            {
                isCancelled |= e.Key == VirtualKey.Escape;

                Focus(FocusState.Pointer);

                e.Handled = true;
            }
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

            if (associatedNode != null)
            {
                if (!isCancelled)
                {
                    associatedNode.Document.MakeTransaction("Change Text", c =>
                    {
                        c.Apply(new ChangeTextCommand(associatedNode, textBox.Text, true));
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
