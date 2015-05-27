﻿// ==========================================================================
// IPopupControl.cs
// Metro Library SE
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using Windows.UI.Xaml.Controls.Primitives;

namespace GreenParrot.Windows.UI
{
    /// <summary>
    /// Implement this interface when you want to display a control as popup to get access to the popup instance.
    /// </summary>
    public interface IPopupControl
    {
        /// <summary>
        /// Gets or sets the current popup.
        /// </summary>
        /// <value>The current popup.</value>
        /// <remarks>This property is assigned immediatly before opening the popup and will be reset when the popup is closed.</remarks>
        Popup Popup { get; set; }
    }
}