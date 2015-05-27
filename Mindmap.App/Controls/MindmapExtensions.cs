// ==========================================================================
// MindmapExtensions.cs
// Mindmap Application
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using Mindmap.Model;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace Mindmap.Controls
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
