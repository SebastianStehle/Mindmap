// ==========================================================================
// MindmapFlyoutView.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using Windows.UI.Xaml.Controls;

namespace Hercules.App.Controls
{
    public abstract class MindmapFlyoutView : UserControl
    {
        public Flyout Flyout { get; set; }

        public virtual void OnOpened()
        {
        }

        public virtual void OnClosed()
        {
        }
    }
}
