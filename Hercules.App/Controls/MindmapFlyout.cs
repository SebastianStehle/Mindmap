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
                MindmapFlyoutPopupView view = Content as MindmapFlyoutPopupView;

                if (view != null)
                {
                    view.OnOpened();
                }
            };

            Closed += (sender, e) =>
            {
                MindmapFlyoutPopupView view = Content as MindmapFlyoutPopupView;

                if (view != null)
                {
                    view.OnClosed();
                }
            };
        }
    }
}
