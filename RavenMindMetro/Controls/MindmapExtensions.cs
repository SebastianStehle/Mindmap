// ==========================================================================
// MindmapExtensions.cs
// RavenMind Application
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using RavenMind.Model;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace RavenMind.Controls
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

        public static void MoveBy(this Mindmap mindmap, Node node, Point offset)
        {
            NodeControl element = mindmap.GetControl(node);

            element.Position = new Point(element.Position.X + offset.X, element.Position.Y + offset.Y);

            foreach (Node child in node.Children)
            {
                MoveBy(mindmap, child, offset);
            }
        }
    }
}
