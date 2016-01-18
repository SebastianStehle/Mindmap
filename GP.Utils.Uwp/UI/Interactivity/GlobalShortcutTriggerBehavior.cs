// ==========================================================================
// GlobalShortcutTriggerBehavior.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using Windows.ApplicationModel;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Markup;

namespace GP.Utils.UI.Interactivity
{
    [ContentProperty(Name = nameof(Actions))]
    public sealed class GlobalShortcutTriggerBehavior : ShortcutTriggerBehaviorBase
    {
        /// <summary>
        /// Called after the behavior is attached to an AssociatedObject.
        /// </summary>
        /// <remarks>Override this to hook up functionality to the AssociatedObject.</remarks>
        protected override void OnAttached()
        {
            if (DesignMode.DesignModeEnabled)
            {
                return;
            }

            CoreWindow currentWindow = Window.Current.CoreWindow;

            currentWindow.KeyDown += corewWindow_KeyDown;
            currentWindow.KeyUp += corewWindow_KeyUp;
        }

        /// <summary>
        /// Called when the behavior is being detached from its AssociatedObject, but before it has actually occurred.
        /// </summary>
        /// <remarks>Override this to unhook functionality from the AssociatedObject.</remarks>
        protected override void OnDetaching()
        {
            if (DesignMode.DesignModeEnabled)
            {
                return;
            }

            CoreWindow currentWindow = Window.Current.CoreWindow;

            currentWindow.KeyDown -= corewWindow_KeyDown;
            currentWindow.KeyUp -= corewWindow_KeyUp;
        }

        private void corewWindow_KeyUp(CoreWindow sender, KeyEventArgs e)
        {
            VirtualKey key = e.VirtualKey;

            if (IsCorrectKey(key))
            {
                Execute(this, null);

                e.Handled = true;
            }
        }

        private void corewWindow_KeyDown(CoreWindow sender, KeyEventArgs e)
        {
            VirtualKey key = e.VirtualKey;

            if (IsCorrectKey(key))
            {
                e.Handled = true;
            }
        }
    }
}
