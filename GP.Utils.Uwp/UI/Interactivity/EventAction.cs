// ==========================================================================
// EventAction.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System;
using Windows.UI.Xaml;
using Microsoft.Xaml.Interactivity;

namespace GP.Utils.UI.Interactivity
{
    public class EventAction : DependencyObject, IAction
    {
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
            OnInvoked(new RoutedEventArgs());

            return null;
        }
    }
}
