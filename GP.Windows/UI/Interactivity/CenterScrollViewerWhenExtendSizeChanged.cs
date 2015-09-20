// ==========================================================================
// CenterScrollViewerWhenExtendSizeChanged.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace GP.Windows.UI.Interactivity
{
    /// <summary>
    /// Centers the scroll viewer.
    /// </summary>
    public sealed class CenterScrollViewerWhenExtendSizeChanged : Behavior<ScrollViewer>
    {
        private DependencyPropertyListener contentListener;
        private FrameworkElement content;

        /// <summary>
        /// Called after the behavior is attached to an AssociatedObject.
        /// </summary>
        /// <remarks>Override this to hook up functionality to the AssociatedObject.</remarks>
        protected override void OnAttached()
        {
            contentListener = new DependencyPropertyListener(AssociatedElement, "Content", ContentChanged);

            Initialize();

            Center();

            base.OnAttached();
        }

        /// <summary>
        /// Called when the behavior is being detached from its AssociatedObject, but before it has actually occurred.
        /// </summary>
        /// <remarks>Override this to unhook functionality from the AssociatedObject.</remarks>
        protected override void OnDetaching()
        {
            contentListener.Release();

            Release();

            base.OnDetaching();
        }

        private void ContentChanged()
        {
            Release();

            Initialize();
        }

        private void Initialize()
        {
            content = AssociatedElement.Content as FrameworkElement;

            if (content != null)
            {
                content.SizeChanged += AssociatedElement_SizeChanged;
            }
        }

        private void Release()
        {
            if (content != null)
            {
                content.SizeChanged -= AssociatedElement_SizeChanged;
            }

            content = null;
        }

        private void AssociatedElement_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            Center();
        }

        private void Center()
        {
            if (content != null && AssociatedElement.ExtentWidth > 0 && AssociatedElement.ExtentHeight > 0 && AssociatedElement.ActualWidth > 0 && AssociatedElement.ActualHeight > 0)
            {
                AssociatedElement.CenterViewport();
            }
        }
    }
}
