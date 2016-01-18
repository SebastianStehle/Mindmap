// ==========================================================================
// ShowFlyoutAction.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Microsoft.Xaml.Interactivity;

namespace GP.Utils.UI.Interactivity
{
    public sealed class ShowFlyoutAction : DependencyObject, IAction
    {
        /// <summary>
        /// Executes the action.
        /// </summary>
        /// <param name="sender">The <see cref="T:System.Object"/> that is passed to the action by the behavior. Generally this is <seealso cref="P:Microsoft.Xaml.Interactivity.IBehavior.AssociatedObject"/> or a target object.</param><param name="parameter">The value of this parameter is determined by the caller.</param>
        /// <remarks>
        /// An example of parameter usage is EventTriggerBehavior, which passes the EventArgs as a parameter to its actions.
        /// </remarks>
        /// <returns>
        /// Returns the result of the action.
        /// </returns>
        public object Execute(object sender, object parameter)
        {
            FrameworkElement element = sender as FrameworkElement;

            if (element == null)
            {
                IBehavior behavior = sender as IBehavior;

                if (behavior != null)
                {
                    element = behavior.AssociatedObject as FrameworkElement;
                }
            }

            if (element == null)
            {
                return null;
            }

            FlyoutBase flyout = FlyoutBase.GetAttachedFlyout(element);

            if (flyout == null)
            {
                Button button = element as Button;

                if (button != null)
                {
                    flyout = button.Flyout;
                }
            }

            if (flyout != null)
            {
                flyout.ShowAt(element);
            }

            return null;
        }
    }
}
