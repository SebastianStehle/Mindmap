// ==========================================================================
// MindmapExtensions.cs
// Mindmap Application
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using MindmapApp.Model;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace MindmapApp.Controls
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
