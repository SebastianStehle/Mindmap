// ==========================================================================
// VisualTreeExtensions.cs
// Metro Library SE
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System;
using Windows.Foundation;
using Windows.System;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;

namespace SE.Metro.UI
{
    /// <summary>
    /// Provides some helper and extension methods to work with the visual tree of controls.
    /// </summary>
    public static class VisualTreeExtensions
    {
        /// <summary>
        /// Defines a point where both values are zero.
        /// </summary>
        public static readonly Point PointZero = new Point(0, 0);
        /// <summary>
        /// Defines a point where both values are not numbers.
        /// </summary>
        public static readonly Point PointNaN = new Point(double.NaN, double.NaN);

        /// <summary>
        /// Centers the view port of the scrollviewer.
        /// </summary>
        /// <param name="scrollViewer">The target scrollviewer. Cannot be null.</param>
        /// <exception cref="System.ArgumentNullException"><paramref name="scrollViewer"/> is null.</exception>
        public static void CenterViewport(this ScrollViewer scrollViewer)
        {
            Guard.NotNull(scrollViewer, "scrollViewer");

            double x = 0.5 * (scrollViewer.ExtentWidth  - scrollViewer.ViewportWidth);
            double y = 0.5 * (scrollViewer.ExtentHeight - scrollViewer.ActualHeight);

            scrollViewer.ChangeView(x, y, null);
        }

        /// <summary>
        /// Determines whether the specified key is a letter or a number.
        /// </summary>
        /// <param name="key">The key to check.</param>
        /// <returns>A value indicating if the specified key is a letter or a number.</returns>
        public static bool IsLetterOrNumber(this VirtualKey key)
        {
            return key >= VirtualKey.Number0 && key <= VirtualKey.Z;
        }

        /// <summary>
        /// Brings the child element into view when the virtual keyboard becomes visible.
        /// </summary>
        /// <param name="scrollViewer">The scrollviewer. Cannot be null.</param>
        /// <param name="eventArgs">The <see cref="InputPaneVisibilityEventArgs" /> instance containing the event data. Cannot be null.</param>
        /// <returns>A value indicating if a child element was occluded.</returns>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="scrollViewer"/> is null.
        ///     - or -
        ///     <paramref name="eventArgs"/> is null.
        /// </exception>
        public static bool BringChildElementIntoView(this ScrollViewer scrollViewer, InputPaneVisibilityEventArgs eventArgs)
        {
            Guard.NotNull(scrollViewer, "scrollViewer");
            Guard.NotNull(eventArgs, "eventArgs");

            UIElement focusedElement = FocusManager.GetFocusedElement() as UIElement;

            if (focusedElement != null && VisualTreeExtensions.IsChild(scrollViewer, focusedElement))
            {
                double occludedHeight = eventArgs.OccludedRect.Height + 88;

                Rect visibleBounds = focusedElement.TransformToVisual(Window.Current.Content).TransformBounds(new Rect(new Point(0, 0), focusedElement.RenderSize));

                double delta = visibleBounds.Bottom + occludedHeight - Window.Current.Bounds.Height;

                if (delta > 0)
                {
                    scrollViewer.ChangeView(null, scrollViewer.VerticalOffset + delta, null);

                    eventArgs.EnsuredFocusedElementInView = true;
                }
            }

            return eventArgs.EnsuredFocusedElementInView;
        }

        /// <summary>
        /// Animates the y-position of the element.
        /// </summary>
        /// <param name="element">The element to animate.</param>
        /// <param name="value">The target y-offset of the element.</param>
        /// <param name="duration">The duration of the animation.</param>
        public static void AnimateY(this UIElement element, double value, TimeSpan duration)
        {
            CompositeTransform transform = element.RenderTransform as CompositeTransform;

            if (transform == null)
            {
                element.RenderTransform = transform = new CompositeTransform();
            }

            DoubleAnimation doubleAnimation = new DoubleAnimation { From = transform.TranslateY, To = value, Duration = duration, EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseInOut } };

            Storyboard.SetTarget(doubleAnimation, element);
            Storyboard.SetTargetProperty(doubleAnimation, "(UIElement.RenderTransform).(CompositeTransform.TranslateY)");

            Storyboard storyboard = new Storyboard { Duration = duration };
            storyboard.Children.Add(doubleAnimation);
            storyboard.Begin();
        }

        /// <summary>
        /// Determines whether the specified child is child control of the target object.
        /// </summary>
        /// <param name="target">The target object, which is a possible parent
        /// of the child object. Cannot be null.</param>
        /// <param name="child">The child object to check. Cannot be null.</param>
        /// <returns>
        /// <c>true</c> if the target is parent of the child control; otherwise, <c>false</c>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="target"/> is null.
        ///     - or -
        ///     <paramref name="child"/> is null.
        /// </exception>
        public static bool IsChild(this DependencyObject target, DependencyObject child)
        {
            Guard.NotNull(target, "target");
            Guard.NotNull(child, "child");

            DependencyObject parent = VisualTreeHelper.GetParent(child);

            if (parent == target)
            {
                return true;
            }
            else if (parent == null)
            {
                return false;
            }

            return IsChild(target, parent);
        }

        /// <summary>
        /// Finds the parent of the target <see cref="DependencyObject"/> with the specified type.
        /// </summary>
        /// <typeparam name="T">The type of the parent to find.</typeparam>
        /// <param name="target">The target where to get the parent from. Cannot be null.</param>
        /// <returns>
        /// The parent of the target object when such a parent exists or null otherwise.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="target"/> is null.</exception>
        public static T FindParent<T>(this DependencyObject target) where T : class
        {
            Guard.NotNull(target, "target");

            for (var temp = VisualTreeHelper.GetParent(target); temp != null; temp = VisualTreeHelper.GetParent(temp))
            {
                var result = temp as T;
                if (result != null)
                {
                    return result;
                }
            }

            return null;
        }
        /// <summary>
        /// Finds the parent of the target <see cref="DependencyObject"/> with the specified type and predicate.
        /// </summary>
        /// <typeparam name="T">The type of the parent to find.</typeparam>
        /// <param name="target">The target where to get the parent from. Cannot be null.</param>
        /// <param name="predicate">The predicate. Cannot be null.</param>
        /// <returns>
        /// The parent of the target object when such a parent exists or null otherwise.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="target"/> is null.
        ///     - or -
        ///     <paramref name="predicate"/> is null.
        /// </exception>
        public static T FindParent<T>(this DependencyObject target, Predicate<T> predicate) where T : class
        {
            Guard.NotNull(target, "target");
            Guard.NotNull(predicate, "predicate");

            for (var temp = VisualTreeHelper.GetParent(target); temp != null; temp = VisualTreeHelper.GetParent(temp))
            {
                T result = temp as T;

                if (result != null && predicate(result))
                {
                    return result;
                }
            }

            return null;
        }

        /// <summary>
        /// Creates a new <see cref="PathGeometry"/> instance that is only initialized 
        /// with a single segment and the specified start position.
        /// </summary>
        /// <param name="startPosition">The start position.</param>
        /// <param name="segment">The single segment of the <see cref="PathGeometry"/>.</param>
        /// <returns>
        /// The resulting <see cref="PathGeometry"/> object.
        /// </returns>
        public static PathGeometry CreatePathGeometryForSingleSegment(Point startPosition, PathSegment segment)
        {
            PathFigure figure = new PathFigure { StartPoint = startPosition, Segments = { segment } };

            PathGeometry geometry =
                new PathGeometry
                {
                    Figures =
                        new PathFigureCollection
                        {
                            figure
                        }
                };

            return geometry;
        }

        /// <summary>
        /// Determines whether an element is in the visual tree of a given ancestor.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="ancestor">The ancestor.</param>
        /// <returns>
        /// <c>true</c> if element paramter is in visual tree otherwise, <c>false</c>.
        /// </returns>
        public static bool IsInVisualTree(this FrameworkElement element, FrameworkElement ancestor)
        {
            FrameworkElement frameworkElement = element;

            while (frameworkElement != null)
            {
                if (frameworkElement == ancestor)
                {
                    return true;
                }

                frameworkElement = VisualTreeHelper.GetParent(frameworkElement) as FrameworkElement;
            }

            return false;
        }

        /// <summary>
        /// Tries the add the element to the panel if this panel existing and has a valid children property.
        /// </summary>
        /// <param name="panel">The panel where to add the element to.</param>
        /// <param name="element">The element to add.</param>
        /// <returns>
        /// The result of the operation.
        /// </returns>
        public static bool TryAdd(this Panel panel, UIElement element)
        {
            bool result = false;

            if (panel != null && panel.Children != null)
            {
                panel.Children.Add(element);

                result = true;
            }

            return result;
        }

        /// <summary>
        /// Tries the remove the element from the panel if this panel existing and has a valid children property.
        /// </summary>
        /// <param name="panel">The panel where to remove the element from.</param>
        /// <param name="element">The element to remove.</param>
        /// <returns>
        /// The result of the operation.
        /// </returns>
        public static bool TryRemove(this Panel panel, UIElement element)
        {
            if (panel != null && panel.Children != null)
            {
                return panel.Children.Remove(element);
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// Calculates the center of the of the y-Coordinate of the target rect.
        /// </summary>
        /// <param name="rect">The rect where to get the center from.</param>
        /// <returns>
        /// The center.
        /// </returns>
        public static double CenterY(this Rect rect)
        {
            return rect.Y + (0.5 * rect.Height);
        } 
    }
}