// ==========================================================================
// ControlHelpers.cs
// Metro Library SE
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace SE.Metro.UI
{
    /// <summary>
    /// Provides some helper methods to deal with controls.
    /// </summary>
    public static class ControlHelpers
    {
        /// <summary>
        /// Use this attached dependency property to indicate of you want to keep the app bar open.
        /// </summary>
        public static readonly DependencyProperty KeepOpenProperty = 
            DependencyProperty.RegisterAttached("KeepOpen", typeof(bool), typeof(ControlHelpers), new PropertyMetadata(false, new PropertyChangedCallback(OnKeepOpenChanged)));
        /// <summary>
        /// Sets the <see cref="KeepOpenProperty"/> attached dependency property for the specified app bar.
        /// </summary>
        /// <param name="element">The element on which to set the property value. Cannot be null.</param>
        /// <param name="value">The value to set.</param>
        /// <exception cref="ArgumentNullException"><paramref name="element"/> is null.</exception>
        public static void SetKeepOpen(AppBar element, bool value)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }

            element.SetValue(KeepOpenProperty, value);
        }

        /// <summary>
        /// Gets the value of the <see cref="KeepOpenProperty"/> attached dependency property from the specified app bar.
        /// </summary>
        /// <param name="element">The element from which the property value is read.. Cannot be null.</param>
        /// <returns>The value of the property.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="element"/> is null.</exception>
        public static bool GetKeepOpen(AppBar element)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }

            return (bool)element.GetValue(KeepOpenProperty);
        }

        private static void OnKeepOpenChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            AppBar appBar = (AppBar)o;

            bool keepOpen = GetKeepOpen(appBar);

            if (keepOpen)
            {
                appBar.Closed += appBar_Closed;
            }
            else
            {
                appBar.Closed -= appBar_Closed;
            }
        }

        private static void appBar_Closed(object sender, object e)
        {
            AppBar appBar = (AppBar)sender;

            appBar.IsOpen = true;
        }
    }
}
