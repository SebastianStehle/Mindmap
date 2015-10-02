// ==========================================================================
// CommandShortcutBehavior.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System;
using System.Windows.Input;
using Windows.ApplicationModel;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace GP.Windows.UI.Interactivity
{
    /// <summary>
    /// A behavior to invoke the command of the button when the key is pressed.
    /// </summary>
    public class CommandShortcutBehavior : Behavior<FrameworkElement>
    {
        private bool isShiftKeyPressed;
        private bool isControlKeyPressed;

        /// <summary>
        /// Occurs when when the behavior is invoking.
        /// </summary>
        public event EventHandler<CommandInvokingEventHandler> Invoking;
        /// <summary>
        /// Raises the <see cref="E:Invoking"/> event.
        /// </summary>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        protected virtual void OnInvoking(CommandInvokingEventHandler e)
        {
            EventHandler<CommandInvokingEventHandler> eventHandler = Invoking;

            if (eventHandler != null)
            {
                eventHandler(this, e);
            }
        }

        /// <summary>
        /// Occurs when when the behavior is invoked.
        /// </summary>
        public event RoutedEventHandler Invoked;
        /// <summary>
        /// Raises the <see cref="E:Invoked"/> event.
        /// </summary>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected virtual void OnInvoked(RoutedEventArgs e)
        {
            RoutedEventHandler eventHandler = Invoked;

            if (eventHandler != null)
            {
                eventHandler(this, e);
            }
        }

        /// <summary>
        /// Defines the <see cref="Key"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty KeyProperty =
            DependencyProperty.Register("Key", typeof(VirtualKey), typeof(CommandShortcutBehavior), new PropertyMetadata(VirtualKey.None));
        /// <summary>
        /// Gets or sets the key that must be pressed when the command should be invoked.
        /// </summary>
        /// <value>The key that must be pressed when the command should be invoked.</value>
        public VirtualKey Key
        {
            get { return (VirtualKey)GetValue(KeyProperty); }
            set { SetValue(KeyProperty, value); }
        }

        /// <summary>
        /// Defines the <see cref="ListenToControl"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ListenToControlProperty =
            DependencyProperty.Register("ListenToControl", typeof(bool), typeof(CommandShortcutBehavior), new PropertyMetadata(false));
        /// <summary>
        /// Gets or sets a value indicating if the keys of the control will be handled.
        /// </summary>
        /// <value>A value indicating if the keys of the control will be handled.</value>
        public bool ListenToControl
        {
            get { return (bool)GetValue(ListenToControlProperty); }
            set { SetValue(ListenToControlProperty, value); }
        }

        /// <summary>
        /// Defines the <see cref="RequiresControlModifier"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty RequiresControlModifierProperty =
            DependencyProperty.Register("RequiresControlModifier", typeof(bool), typeof(CommandShortcutBehavior), new PropertyMetadata(false));
        /// <summary>
        /// Gets or sets a value indicating if the control key must be pressed.
        /// </summary>
        /// <value>A value indicating if the control key must be pressed.</value>
        public bool RequiresControlModifier
        {
            get { return (bool)GetValue(RequiresControlModifierProperty); }
            set { SetValue(RequiresControlModifierProperty, value); }
        }

        /// <summary>
        /// Defines the <see cref="RequiresShiftModifier"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty RequiresShiftModifierProperty =
            DependencyProperty.Register("RequiresShiftModifier", typeof(bool), typeof(CommandShortcutBehavior), new PropertyMetadata(false));
        /// <summary>
        /// Gets or sets a value indicating if the shift key must be pressed.
        /// </summary>
        /// <value>A value indicating if the shift key must be pressed.</value>
        public bool RequiresShiftModifier
        {
            get { return (bool)GetValue(RequiresShiftModifierProperty); }
            set { SetValue(RequiresShiftModifierProperty, value); }
        }

        /// <summary>
        /// Defines the <see cref="Command"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.Register("Command", typeof(ICommand), typeof(CommandShortcutBehavior), new PropertyMetadata(null));
        /// <summary>
        /// Gets or sets the command to invoke.
        /// </summary>
        /// <value>The command to invoke.</value>
        public ICommand Command
        {
            get { return (ICommand)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }

        /// <summary>
        /// Defines the <see cref="CommandParameter"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CommandParameterProperty =
            DependencyProperty.Register("CommandParameter", typeof(object), typeof(CommandShortcutBehavior), new PropertyMetadata(null));
        /// <summary>
        /// Gets or sets parameter for the command to invoke.
        /// </summary>
        /// <value>The parameter for the command to invoke.</value>
        public object CommandParameter
        {
            get { return GetValue(CommandParameterProperty); }
            set { SetValue(CommandParameterProperty, value); }
        }

        /// <summary>
        /// Called after the behavior is attached to an AssociatedObject.
        /// </summary>
        /// <remarks>Override this to hook up functionality to the AssociatedObject.</remarks>
        protected override void OnAttached()
        {
            AssociatedElement.KeyDown += AssociatedElement_KeyDown;
            AssociatedElement.KeyUp   += AssociatedElement_KeyUp;

            if (!DesignMode.DesignModeEnabled)
            {
                CoreWindow currentWindow = CoreWindow.GetForCurrentThread();

                currentWindow.KeyDown += corewWindow_KeyDown;
                currentWindow.KeyUp   += corewWindow_KeyUp;
            }
        }

        /// <summary>
        /// Called when the behavior is being detached from its AssociatedObject, but before it has actually occurred.
        /// </summary>
        /// <remarks>Override this to unhook functionality from the AssociatedObject.</remarks>
        protected override void OnDetaching()
        {
            AssociatedElement.KeyDown -= AssociatedElement_KeyDown;
            AssociatedElement.KeyUp   -= AssociatedElement_KeyUp;

            if (!DesignMode.DesignModeEnabled)
            {
                CoreWindow currentWindow = CoreWindow.GetForCurrentThread();

                currentWindow.KeyDown -= corewWindow_KeyDown;
                currentWindow.KeyUp   -= corewWindow_KeyUp;
            }
        }

        private void AssociatedElement_KeyUp(object sender, KeyRoutedEventArgs e)
        {
            VirtualKey key = e.Key;

            if (ListenToControl && IsCorrectKey(key))
            {
                Invoke();

                e.Handled = true;
            }
        }

        private void AssociatedElement_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            VirtualKey key = e.Key;

            if (ListenToControl && IsCorrectKey(key))
            {
                e.Handled = true;
            }
        }

        private void corewWindow_KeyUp(CoreWindow sender, KeyEventArgs e)
        {
            VirtualKey key = e.VirtualKey;

            if (key == VirtualKey.Shift)
            {
                isShiftKeyPressed = false;
            }
            else if (key == VirtualKey.Control)
            {
                isControlKeyPressed = false;
            }
            else if (!ListenToControl && IsCorrectKey(key))
            {
                Invoke();

                e.Handled = true;
            }
        }

        private void corewWindow_KeyDown(CoreWindow sender, KeyEventArgs e)
        {
            VirtualKey key = e.VirtualKey;

            if (key == VirtualKey.Shift)
            {
                isShiftKeyPressed = true;
            }
            else if (key == VirtualKey.Control)
            {
                isControlKeyPressed = true;
            }
            else if (!ListenToControl && IsCorrectKey(key))
            {
                e.Handled = true;
            }
        }

        private bool IsCorrectKey(VirtualKey key)
        {
            return key == Key && (isShiftKeyPressed || !RequiresShiftModifier) && (isControlKeyPressed || !RequiresControlModifier);
        }

        private void Invoke()
        {
            object parameter = CommandParameter;

            ICommand command = Command;

            if (command == null)
            {
                Button button = AssociatedObject as Button;

                if (button?.Command != null)
                {
                    command = button.Command;

                    parameter = button.CommandParameter;
                }
            }

            if (command != null)
            {
                CommandInvokingEventHandler eventArgs = new CommandInvokingEventHandler();

                OnInvoking(eventArgs);

                if (!eventArgs.Handled)
                {
                    if (command.CanExecute(parameter))
                    {
                        command.Execute(parameter);
                    }
                }
            }
            else
            {
                OnInvoked(new RoutedEventArgs());
            }
        }
    }
}
