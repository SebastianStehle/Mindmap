// ==========================================================================
// MindmapExtensions.cs
// Mindmap Application
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using Windows.UI.Xaml.Media;

namespace Hercules.App.Controls
{
    public static class MindmapExtensions
    {
        public static void MakePanelAnimated(this NodeControl nodeControl, bool isAnimating)
        {
            MindmapPanel panel = VisualTreeHelper.GetParent(nodeControl) as MindmapPanel;

            if (panel != null)
            {
                panel.IsAnimating = isAnimating;
            }
        }
    }
}
