// ==========================================================================
// ShortcutEventTriggerBehaviorBase.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System.Diagnostics;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Markup;
using Microsoft.Xaml.Interactions.Core;
using Microsoft.Xaml.Interactivity;

namespace GP.Utils.UI.Interactivity
{
    /// <summary>
    /// Base class for all shortcuts.
    /// </summary>
    [ContentProperty(Name = nameof(Actions))]
    public abstract class ShortcutTriggerBehaviorBase : Behavior<DependencyObject>
    {
        /// <summary>
        /// Defines the <see cref="Key"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty KeyProperty =
            DependencyProperty.Register(nameof(Key), typeof(VirtualKey), typeof(ShortcutTriggerBehaviorBase), new PropertyMetadata(VirtualKey.None));
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
        /// Defines the <see cref="RequiresControlModifier"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty RequiresControlModifierProperty =
            DependencyProperty.Register(nameof(RequiresControlModifier), typeof(bool), typeof(ShortcutTriggerBehaviorBase), new PropertyMetadata(false));
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
            DependencyProperty.Register(nameof(RequiresShiftModifier), typeof(bool), typeof(ShortcutTriggerBehaviorBase), new PropertyMetadata(false));
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
        /// Identifies the <seealso cref="Actions"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ActionsProperty =
            DependencyProperty.Register(nameof(Actions), typeof(ActionCollection), typeof(ShortcutTriggerBehaviorBase), new PropertyMetadata(null));
        /// <summary>
        /// Gets the collection of actions associated with the behavior. This is a dependency property.
        /// </summary>
        public ActionCollection Actions
        {
            get
            {
                ActionCollection actionCollection = (ActionCollection)GetValue(EventTriggerBehavior.ActionsProperty);

                if (actionCollection == null)
                {
                    actionCollection = new ActionCollection();

                    SetValue(EventTriggerBehavior.ActionsProperty, actionCollection);
                }

                return actionCollection;
            }
        }

        /// <summary>
        /// Executes all actions in the <see cref="ActionCollection"/> and returns their results.
        /// </summary>
        /// <param name="sender">The <see cref="object"/> which will be passed on to the action.</param>
        /// <param name="parameter">The value of this parameter is determined by the calling behavior.</param>
        /// <returns>Returns the results of the actions.</returns>
        protected void Execute(object sender, object parameter)
        {
            Interaction.ExecuteActions(sender, Actions, parameter);
        }

        /// <summary>
        /// Determines whether the specified key is the correct key.
        /// </summary>
        /// <param name="key">The key to check.</param>
        /// <returns>
        /// True, if the key is correct or false otherwise.
        /// </returns>
        protected bool IsCorrectKey(VirtualKey key)
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
    }
}
