// ==========================================================================
// Interactions.cs
// Metro Library SE
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System;
using Windows.UI.Xaml;

namespace SE.Metro.UI.Interactivity
{
    /// <summary>
    /// Static class that owns the Adorners attached properties. 
    /// Handles propagation of AssociatedObject change notifications. 
    /// </summary>
    public static class Interactions
    {
        /// <summary>
        ///  This property is used as the internal backing store for the public Behaviors attached property.
        /// </summary>
        public static readonly DependencyProperty AdornersProperty =
            DependencyProperty.RegisterAttached("Adorners", typeof(AdornerCollection), typeof(Interactions), new PropertyMetadata(null, new PropertyChangedCallback(OnAdornersChanged)));
        /// <summary>
        /// Gets the adorners for the specified object.
        /// </summary>
        /// <param name="obj">The object to get the adorners for. Cannot be null.</param>
        /// <returns>The adorners. Will never be null.</returns>
        public static AdornerCollection GetAdorners(FrameworkElement obj)
        {
            if (obj == null)
            {
                throw new ArgumentNullException("obj");
            }

            AdornerCollection adorners = (AdornerCollection)obj.GetValue(AdornersProperty);

            if (adorners == null)
            {
                adorners = new AdornerCollection();

                obj.SetValue(AdornersProperty, adorners);
            }

            return adorners;
        } 

        private static void OnAdornersChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            ManageCollection(obj, e);
        }

        /// <summary>
        ///  This property is used as the internal backing store for the public Behaviors attached property.
        /// </summary>
        public static readonly DependencyProperty BehaviorsProperty =
            DependencyProperty.RegisterAttached("Behaviors", typeof(BehaviorCollection), typeof(Interactions), new PropertyMetadata(null, new PropertyChangedCallback(OnBehaviorsChanged)));
        /// <summary>
        /// Gets the behaviors for the specified object.
        /// </summary>
        /// <param name="obj">The object to get the behaviors for. Cannot be null.</param>
        /// <returns>The behaviors. Will never be null.</returns>
        public static BehaviorCollection GetBehaviors(FrameworkElement obj)
        {
            if (obj == null)
            {
                throw new ArgumentNullException("obj");
            }

            BehaviorCollection behaviors = (BehaviorCollection)obj.GetValue(BehaviorsProperty);

            if (behaviors == null)
            {
                behaviors = new BehaviorCollection();

                obj.SetValue(BehaviorsProperty, behaviors);
            }

            return behaviors;
        }

        private static void OnBehaviorsChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            ManageCollection(obj, e);
        }

        /// <summary>
        /// Attachs the adorners to the specified framework element.
        /// </summary>
        /// <param name="frameworkElement">The object to attach the objects to. Cannot be null.</param>
        /// <param name="adorners">The adorners to attach to the framework element. Cannot be null.</param>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="frameworkElement"/> is null.
        ///     - or -
        ///     <paramref name="adorners"/> is null.
        /// </exception>
        public static void AttachAdorners(FrameworkElement frameworkElement, params Adorner[] adorners)
        {
            if (frameworkElement == null)
            {
                throw new ArgumentNullException("frameworkElement");
            }

            if (adorners == null)
            {
                throw new ArgumentNullException("adorners");
            }

            AdornerCollection adornerCollection = GetAdorners(frameworkElement);

            adornerCollection.AddRange(adorners);
        }

        /// <summary>
        /// Attachs the behaviors to the specified framework element.
        /// </summary>
        /// <param name="frameworkElement">The object to attach the objects to. Cannot be null.</param>
        /// <param name="behaviors">The behaviors to attach to the framework element. Cannot be null.</param>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="frameworkElement"/> is null.
        ///     - or -
        ///     <paramref name="behaviors"/> is null.
        /// </exception>
        public static void AttachBehaviors(FrameworkElement frameworkElement, params Behavior[] behaviors)
        {
            if (frameworkElement == null)
            {
                throw new ArgumentNullException("frameworkElement");
            }

            if (behaviors == null)
            {
                throw new ArgumentNullException("behaviors");
            }

            BehaviorCollection behaviorCollection = GetBehaviors(frameworkElement);

            behaviorCollection.AddRange(behaviors);
        }

        private static void ManageCollection(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            IAttachedObject oldValue = (IAttachedObject)e.OldValue;
            IAttachedObject newValue = (IAttachedObject)e.NewValue;

            if (oldValue != newValue)
            {
                if (oldValue != null && oldValue.AssociatedObject != null)
                {
                    oldValue.Detach();
                }
                if (newValue != null && obj != null)
                {
                    if (newValue.AssociatedObject != null)
                    {
                        throw new InvalidOperationException("Cannot assign the adorners to multiple elements.");
                    }

                    newValue.Attach((FrameworkElement)obj);
                }
            }
        }
    }
}
