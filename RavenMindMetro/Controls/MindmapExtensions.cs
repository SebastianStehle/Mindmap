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

namespace RavenMind.Controls
{
    public static class MindmapExtensions
    {
        public static void BringFront(this UIElement nodeControl)
        {
            Canvas.SetZIndex(nodeControl, 1000);
        }

        public static void BringBack(this UIElement nodeControl)
        {
            Canvas.SetZIndex(nodeControl, 0);
        }

        public static void MoveNodeToPosition(this Mindmap mindmap, Node node, Point newPosition, bool animated)
        {
            NodeControl element = mindmap.GetControl(node);

            Point position = element.Position;

            Point offset = new Point();

            offset.X += newPosition.X - position.X;
            offset.Y += newPosition.Y - position.Y;

            MoveNode(mindmap, node, offset, animated);
        }

        public static void MoveNode(this Mindmap mindmap, Node node, Point offset, bool animated)
        {
            NodeControl element = mindmap.GetControl(node);

            Point position = element.Position;

            position.X += offset.X;
            position.Y += offset.Y;

            if (position != element.Position)
            {
                element.SetPosition(position, animated);

                foreach (Node child in node.Children)
                {
                    MoveNode(mindmap, child, offset, animated);
                }
            }
        }
    }
}
