// ==========================================================================
// FlyoutShortcutBehavior.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;

namespace GP.Windows.UI.Interactivity
{
    /// <summary>
    /// A behavior to show the attached flyout when the shortcut key is pressed.
    /// </summary>
    public class FlyoutShortcutBehavior : ShortcutBehaviorBase
    {
        /// <summary>
        /// Called when the shortcut must be invoked.
        /// </summary>
        protected override void InvokeShortcut()
        {
            FlyoutBase flyout = FlyoutBase.GetAttachedFlyout(AssociatedElement);

            if (flyout == null)
            {
                Button button = AssociatedElement as Button;

                if (button != null)
                {
                    flyout = button.Flyout;
                }
            }

            if (flyout != null)
            {
                flyout.ShowAt(AssociatedElement);
            }
        }
    }
}
