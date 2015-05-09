// ==========================================================================
// ScrollViewerBringIntoViewBehavior.cs
// RavenMind Application
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace SE.Metro.UI.Interactivity
{
    /// <summary>
    /// Behavior that brings the focused element into view when the input pane becomes visible.
    /// </summary>
    public sealed class ScrollViewerBringIntoViewBehavior : Behavior<FrameworkElement>
    {
        /// <summary>
        /// Called after the behavior is attached to an AssociatedObject.
        /// </summary>
        /// <remarks>Override this to hook up functionality to the AssociatedObject.</remarks>
        protected override void OnAttached()
        {
            GotFocus += ScrollViewerBringIntoViewBehavior_GotFocus;
        }

        /// <summary>
        /// Called when the behavior is being detached from its AssociatedObject, but before it has actually occurred.
        /// </summary>
        /// <remarks>Override this to unhook functionality from the AssociatedObject.</remarks>
        protected override void OnDetaching()
        {
            GotFocus -= ScrollViewerBringIntoViewBehavior_GotFocus;
        }

        private void ScrollViewerBringIntoViewBehavior_GotFocus(object sender, RoutedEventArgs e)
        {
            ScrollViewer parent = VisualTreeExtensions.FindParent<ScrollViewer>(AssociatedObject);

            if (parent != null)
            {
                Rect visibleBounds = AssociatedObject.TransformToVisual(Window.Current.Content).TransformBounds(new Rect(new Point(0, 0), AssociatedObject.RenderSize));

                if (AssociatedObject.RenderSize.Width < parent.RenderSize.Width && AssociatedObject.RenderSize.Height < parent.RenderSize.Height)
                {
                    double dx = 0;
                    double dy = 0;

                    if (visibleBounds.Left < 0)
                    {
                        dx = visibleBounds.Left;
                    }
                    else if (visibleBounds.Right > parent.RenderSize.Width)
                    {
                        dx = visibleBounds.Right - parent.RenderSize.Width;
                    }

                    if (visibleBounds.Top < 0)
                    {
                        dy = visibleBounds.Top;
                    }
                    else if (visibleBounds.Bottom > parent.RenderSize.Height)
                    {
                        dy = visibleBounds.Bottom - parent.RenderSize.Height;
                    }

                    parent.ScrollToVerticalOffset(parent.VerticalOffset + dy);
                    parent.ScrollToHorizontalOffset(parent.HorizontalOffset + dx);
                }
            }
        }
    }
}
