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
using SE.Metro.UI.Interactivity;
using System;
using Windows.Foundation;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;

namespace RavenMind.Controls
{
    [TemplatePart(Name = PartTextBox, Type = typeof(TextBox))]
    [TemplateVisualState(Name = StateNameNormal, GroupName = "CommonStates")]
    [TemplateVisualState(Name = StateNameHighlighted, GroupName = "CommonStates")]
    public class NodeControl : LoadableControl, INodeView
    {
        #region Constants

        private const string StateNameNormal = "Normal";
        private const string StateNameHighlighted = "Highlighted";
        private const string PartTextBox = "TextBox";

        #endregion

        #region Fields

        private readonly CompositeTransform transform = new CompositeTransform();
        private TextBox textBox;
        private string oldText;

        #endregion

        #region Properties

        public NodeBase AssociatedNode { get; private set; }

        public Size Size
        {
            get
            {
                return new Size(ActualWidth, ActualHeight);
            }
        }

        #endregion

        #region Events

        public event EventHandler PositionChanged;
        protected virtual void OnPositionChanged(EventArgs e)
        {
            EventHandler eventHandler = PositionChanged;

            if (eventHandler != null)
            {
                eventHandler(this, e);
            }
        }

        #endregion

        #region Dependency Properties

        public static readonly DependencyProperty IsTextEditingProperty =
            DependencyProperty.Register("IsTextEditing", typeof(bool), typeof(NodeControl), new PropertyMetadata(false));
        public bool IsTextEditing
        {
            get { return (bool)GetValue(IsTextEditingProperty); }
            set { SetValue(IsTextEditingProperty, value); }
        }

        public static readonly DependencyProperty ScaleProperty =
            DependencyProperty.Register("Scale", typeof(double), typeof(NodeControl), new PropertyMetadata(1d, new PropertyChangedCallback(OnScaleChanged)));
        public double Scale
        {
            get { return (double)GetValue(ScaleProperty); }
            set { SetValue(ScaleProperty, value); }
        }

        private static void OnScaleChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            var owner = o as NodeControl;
            if (owner != null)
            {
                owner.OnScaleChanged();
            }
        }

        private void OnScaleChanged()
        {
            Transform();
        }

        public static readonly DependencyProperty PositionProperty =
            DependencyProperty.Register("Position", typeof(Point), typeof(NodeControl), new PropertyMetadata(VisualTreeExtensions.PointNaN, new PropertyChangedCallback(OnPositionChanged)));
        public Point Position
        {
            get { return (Point)GetValue(PositionProperty); }
            set { SetValue(PositionProperty, value); }
        }

        private static void OnPositionChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            var owner = o as NodeControl;
            if (owner != null)
            {
                owner.OnPositionChanged();
            }
        }

        private void OnPositionChanged()
        {
            OnPositionChanged(EventArgs.Empty);

            Transform();
        }

        public static readonly DependencyProperty IsHighlightedProperty =
            DependencyProperty.Register("IsHighlighted", typeof(bool), typeof(NodeControl), new PropertyMetadata(false, new PropertyChangedCallback(OnHighlighted)));
        public bool IsHighlighted
        {
            get { return (bool)GetValue(IsHighlightedProperty); }
            set { SetValue(IsHighlightedProperty, value); }
        }

        private static void OnHighlighted(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            var owner = o as NodeControl;
            if (owner != null)
            {
                owner.OnHighlighted();
            }
        }

        private void OnHighlighted()
        {
            if (IsHighlighted)
            {
                VisualStateManager.GoToState(this, StateNameHighlighted, true);
            }
            else
            {
                VisualStateManager.GoToState(this, StateNameNormal, true);
            }
        }
        
        #endregion

        #region Constructors

        public NodeControl()
        {
            DefaultStyleKey = typeof(NodeControl);

            Interactions.AttachBehaviors(this, new NodeMovingBehavior(), new ScrollViewerBringIntoViewBehavior());

            RenderTransformOrigin = new Point(0.5, 0.5);
            RenderTransform = transform;
            
            SizeChanged += new SizeChangedEventHandler(NodeControl_SizeChanged);
        }

        #endregion

        #region Methods

        public void Attach(NodeBase node)
        {
            DataContext = node;

            AssociatedNode = node;
            AssociatedNode.SelectionChanged += node_SelectionChanged;
        }

        public void Detach()
        {
            DataContext = null;

            AssociatedNode.SelectionChanged -= node_SelectionChanged;
            AssociatedNode = null;
        }

        private void node_SelectionChanged(object sender, EventArgs e)
        {
            if (AssociatedNode.IsSelected)
            {
                Focus(FocusState.Pointer);
            }
        }

        protected override void OnApplyTemplate()
        {
            textBox = (TextBox)GetTemplateChild(PartTextBox);
            textBox.LostFocus += textBox_LostFocus;
            textBox.GotFocus += textBox_GotFocus;
            textBox.KeyDown += textBox_KeyDown;
        }

        protected override void OnLoaded()
        {
            SetBinding(ManipulationModeProperty, new Binding { Path = new PropertyPath("IsSelected"), Source = DataContext, Converter = new BooleanToTranslationManipulationConverter() });
        }

        private void NodeControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (IsTextEditing)
            {
                Point position = Position;

                Node normalNode = DataContext as Node;

                if (normalNode != null)
                {
                    if (normalNode.Side == NodeSide.Left)
                    {
                        position.X -= e.NewSize.Width - e.PreviousSize.Width;
                    }
                }
                else
                {
                    position.X -= 0.5 * (e.NewSize.Width - e.PreviousSize.Width);
                }

                Position = position;
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
                    textBox.Text = oldText;
                }

                Focus(FocusState.Pointer);

                e.Handled = true;
            }
        }

        protected override void OnDoubleTapped(DoubleTappedRoutedEventArgs e)
        {
            textBox.Focus(FocusState.Pointer);
        }

        protected override void OnTapped(TappedRoutedEventArgs e)
        {
            AssociatedNode.IsSelected = true;
        }

        private void textBox_GotFocus(object sender, RoutedEventArgs e)
        {
            AssociatedNode.Document.BeginTransaction("Change Text");

            if (!string.IsNullOrWhiteSpace(textBox.Text))
            {
                textBox.Select(textBox.Text.Length, 1);
            }

            oldText = textBox.Text;

            this.BringFront();

            IsTextEditing = true;
        }

        private void textBox_LostFocus(object sender, RoutedEventArgs e)
        {
            AssociatedNode.Document.CommitTransaction();

            this.BringBack();

            IsTextEditing = false;
        }

        public void CalculateCenterLeft(ref Point point)
        {
            Point position = Position;

            point.X = position.X;
            point.Y = position.Y + (ActualHeight * 0.5);
        }

        public void CalculateCenterRight(ref Point point)
        {
            Point position = Position;

            point.X = position.X + ActualWidth;
            point.Y = position.Y + (ActualHeight * 0.5);
        }

        public double CalculateCenterX()
        {
            return Position.X + (ActualWidth * 0.5);
        }

        public void SetScale(double scale, bool animated)
        {
            if (animated)
            {
                TimeSpan duration = TimeSpan.FromSeconds(0.5);

                DoubleAnimation scaleAnimation = new DoubleAnimation { To = scale, Duration = duration, EnableDependentAnimation = true, EasingFunction = new CubicEase { EasingMode = EasingMode.EaseIn } };

                Storyboard.SetTarget(scaleAnimation, this);
                Storyboard.SetTargetProperty(scaleAnimation, "Scale");
                Storyboard storyboard = new Storyboard { Duration = duration };
                storyboard.Children.Add(scaleAnimation);
                storyboard.Begin();
            }
            else
            {
                Scale = scale;
            }
        }

        public void SetPosition(Point position, bool animated)
        {
            Point currentPosition = Position;
            
            if (animated && !double.IsNaN(currentPosition.X) && !double.IsNaN(currentPosition.Y))
            {
                EasingFunctionBase easing = new BackEase { EasingMode = EasingMode.EaseOut, Amplitude = 0.3 };

                TimeSpan duration = TimeSpan.FromSeconds(0.4);

                PointAnimation pointAnimation = new PointAnimation { From = currentPosition, To = position, Duration = duration, EnableDependentAnimation = true, EasingFunction = easing };

                Storyboard.SetTarget(pointAnimation, this);
                Storyboard.SetTargetProperty(pointAnimation, "Position");
                Storyboard storyboard = new Storyboard { Duration = duration };
                storyboard.Children.Add(pointAnimation);
                storyboard.Begin();
            }
            else
            {
                Position = position;
            }
        }

        private void Transform()
        {
            Point position = Position;

            transform.TranslateX = position.X;
            transform.TranslateY = position.Y;

            double scale = Scale;

            transform.ScaleX = scale;
            transform.ScaleX = scale;
        }

        #endregion
    }
}
