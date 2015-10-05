// ==========================================================================
// CommandShortcutBehavior.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System;
using System.Diagnostics;
using Windows.ApplicationModel;
using Windows.Security.ExchangeActiveSyncProvisioning;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;

namespace GP.Windows.UI.Interactivity
{
    /// <summary>
    /// A behavior to make an invocation when the shortcut key is pressed.
    /// </summary>
    public abstract class ShortcutBehaviorBase : Behavior<FrameworkElement> 
    {
        /// <summary>
        /// Occurs when when the behavior is invoking.
        /// </summary>
        public event EventHandler<ShortcutInvokingEventHandler> Invoking;
        /// <summary>
        /// Raises the <see cref="E:Invoking"/> event.
        /// </summary>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        protected virtual void OnInvoking(ShortcutInvokingEventHandler e)
        {
            EventHandler<ShortcutInvokingEventHandler> eventHandler = Invoking;

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
            DependencyProperty.Register("Key", typeof(VirtualKey), typeof(ShortcutBehaviorBase), new PropertyMetadata(VirtualKey.None));
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
            DependencyProperty.Register("ListenToControl", typeof(bool), typeof(ShortcutBehaviorBase), new PropertyMetadata(false));
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
            DependencyProperty.Register("RequiresControlModifier", typeof(bool), typeof(ShortcutBehaviorBase), new PropertyMetadata(false));
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
            DependencyProperty.Register("RequiresShiftModifier", typeof(bool), typeof(ShortcutBehaviorBase), new PropertyMetadata(false));
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
        /// Called when the shortcut must be invoked.
        /// </summary>
        protected abstract void InvokeShortcut();

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
                CoreWindow currentWindow = Window.Current.CoreWindow;

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
                CoreWindow currentWindow = Window.Current.CoreWindow;

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

            if (!ListenToControl && IsCorrectKey(key))
            {
                Invoke();

                e.Handled = true;
            }
        }

        private void corewWindow_KeyDown(CoreWindow sender, KeyEventArgs e)
        {
            VirtualKey key = e.VirtualKey;

            if (!ListenToControl && IsCorrectKey(key))
            {
                e.Handled = true;
            }
        }

        private bool IsCorrectKey(VirtualKey key)
        {
            return key == Key && (key != VirtualKey.Tab || !IsInSimulator()) && (IsShiftKeyPressed() == RequiresShiftModifier) && (IsControlKeyPressed() == RequiresControlModifier);
        }

        private static bool IsInSimulator()
        {
            return Debugger.IsAttached;
        }

        private static bool IsControlKeyPressed()
        {
            CoreVirtualKeyStates state = Window.Current.CoreWindow.GetKeyState(VirtualKey.Control);

            return state.HasFlag(CoreVirtualKeyStates.Down);
        }

        private static bool IsShiftKeyPressed()
        {
            CoreVirtualKeyStates state = Window.Current.CoreWindow.GetKeyState(VirtualKey.Shift);

            return state.HasFlag(CoreVirtualKeyStates.Down);
        }

        private void Invoke()
        {
            if (IsEnabled())
            {
                ShortcutInvokingEventHandler eventArgs = new ShortcutInvokingEventHandler();

                OnInvoking(eventArgs);

                if (!eventArgs.Handled)
                {
                    InvokeShortcut();
                }

                OnInvoked(new RoutedEventArgs());
            }
        }

        private bool IsEnabled()
        {
            DependencyObject target = AssociatedElement;

            while (target != null)
            {
                Control control = AssociatedElement as Control;

                if (control != null && !control.IsEnabled)
                {
                    return false;
                }

                UIElement element = target as UIElement;

                if (element != null && (element.Visibility == Visibility.Collapsed || Math.Abs(element.Opacity) < float.Epsilon))
                {
                    return false;
                }

                target = VisualTreeHelper.GetParent(target);
            }

            return true;
        }
    }
}
