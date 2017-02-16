// ==========================================================================
// MindmapFlyout.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using Windows.UI.Xaml.Controls;

// ReSharper disable InvertIf

namespace Hercules.App.Controls
{
    public class MindmapFlyout : Flyout
    {
        public MindmapFlyout()
        {
            Opened += (sender, e) =>
            {
                var view = Content as MindmapFlyoutView;

                if (view != null)
                {
                    view.Flyout = this;

                    view.OnOpened();
                }
            };

            Closed += (sender, e) =>
            {
                var view = Content as MindmapFlyoutView;

                if (view != null)
                {
                    view.OnClosed();

                    view.Flyout = null;
                }
            };
        }
    }
}
