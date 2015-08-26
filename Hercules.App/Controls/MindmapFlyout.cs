// ==========================================================================
// MindmapFlyout.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using Windows.UI.Xaml.Controls;

namespace Hercules.App.Controls
{
    public class MindmapFlyout : Flyout
    {
        public MindmapFlyout()
        {
            Opened += (sender, e) =>
            {
                MindmapFlyoutView view = Content as MindmapFlyoutView;

                if (view != null)
                {
                    view.OnOpened();
                }
            };

            Opening += (sender, e) =>
            {
                MindmapFlyoutView view = Content as MindmapFlyoutView;

                if (view != null)
                {
                    view.OnOpening();
                }
            };

            Closed += (sender, e) =>
            {
                MindmapFlyoutView view = Content as MindmapFlyoutView;

                if (view != null)
                {
                    view.OnClosed();
                }
            };
        }
    }
}
