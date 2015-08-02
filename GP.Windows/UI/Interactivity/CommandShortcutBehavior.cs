// ==========================================================================
// CommandShortcutBehavior.cs
// Green Parrot Windows
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System;
using Windows.ApplicationModel;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

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
            get
            {
                return (VirtualKey)GetValue(KeyProperty);
            }
            set
            {
                SetValue(KeyProperty, value);
            }
        }

        /// <summary>
        /// Defines the <see cref="RequiresControlModifier"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty RequiresControlModifierProperty =
            DependencyProperty.Register("RequiresControlModifier", typeof(bool), typeof(CommandShortcutBehavior), new PropertyMetadata(false));
        /// <summary>
        /// Gets or sets a value indicating if the control key must be pressed.
        /// </summary>
        /// <value>A value indicating if the control key must be pressed..</value>
        public bool RequiresControlModifier
        {
            get
            {
                return (bool)GetValue(RequiresControlModifierProperty);
            }
            set
            {
                SetValue(RequiresControlModifierProperty, value);
            }
        }

        /// <summary>
        /// Defines the <see cref="RequiresShiftModifier"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty RequiresShiftModifierProperty =
            DependencyProperty.Register("RequiresShiftModifier", typeof(bool), typeof(CommandShortcutBehavior), new PropertyMetadata(false));
        /// <summary>
        /// Gets or sets a value indicating if the shift key must be pressed.
        /// </summary>
        /// <value>A value indicating if the shift key must be pressed..</value>
        public bool RequiresShiftModifier
        {
            get
            {
                return (bool)GetValue(RequiresShiftModifierProperty);
            }
            set
            {
                SetValue(RequiresShiftModifierProperty, value);
            }
        }

        /// <summary>
        /// Called after the behavior is attached to an AssociatedObject.
        /// </summary>
        /// <remarks>Override this to hook up functionality to the AssociatedObject.</remarks>
        protected override void OnAttached()
        {
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
            if (!DesignMode.DesignModeEnabled)
            {
                CoreWindow currentWindow = CoreWindow.GetForCurrentThread();

                currentWindow.KeyDown -= corewWindow_KeyDown;
                currentWindow.KeyUp   -= corewWindow_KeyUp;
            }
        }

        private void corewWindow_KeyUp(CoreWindow sender, KeyEventArgs args)
        {
            if (args.VirtualKey == VirtualKey.Shift)
            {
                isShiftKeyPressed = false;
            }
            else if (args.VirtualKey == VirtualKey.Control)
            {
                isControlKeyPressed = false;
            }
            else if (args.VirtualKey == Key && (isShiftKeyPressed || !RequiresShiftModifier) && (isControlKeyPressed || !RequiresControlModifier))
            {
                Button button = AssociatedObject as Button;

                CommandInvokingEventHandler eventArgs = new CommandInvokingEventHandler();

                if (button != null && button.Command != null)
                {
                    OnInvoking(eventArgs);

                    if (!eventArgs.Handled)
                    {
                        if (button.Command.CanExecute(button.CommandParameter))
                        {
                            button.Command.Execute(button.CommandParameter);
                        }
                    }
                }
                else
                {
                    OnInvoked(new RoutedEventArgs());
                }

                args.Handled = true;
            }
        }

        private void corewWindow_KeyDown(CoreWindow sender, KeyEventArgs args)
        {
            if (args.VirtualKey == VirtualKey.Shift)
            {
                isShiftKeyPressed = true;
            }
            else if (args.VirtualKey == VirtualKey.Control)
            {
                isControlKeyPressed = true;
            }
        }
    }
}
