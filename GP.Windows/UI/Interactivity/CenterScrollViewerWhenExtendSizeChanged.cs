using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;

namespace GP.Windows.UI.Interactivity
{
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

            base.OnAttached();
        }

        protected override void OnDetaching()
        {
            AssociatedElement.SizeChanged -= AssociatedElement_SizeChanged;

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

            Center();
        }

        private void Release()
        {
            if (content != null)
            {
                content.SizeChanged -= AssociatedElement_SizeChanged;
            }
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

        public sealed class DependencyPropertyListener : DependencyObject
        {
            private readonly Action changed;

            private static readonly DependencyProperty ListenerProperty = 
                DependencyProperty.Register("Listener", typeof(object), typeof(DependencyPropertyListener), new PropertyMetadata(null, OnSourceChanged));

            public DependencyPropertyListener(DependencyObject source, string property, Action changed)
                : this(source, new PropertyPath(property), changed)
            {
            }

            public DependencyPropertyListener(DependencyObject source, PropertyPath property, Action changed)
            {
                Guard.NotNull(changed, nameof(changed));
                Guard.NotNull(source, nameof(source));
                Guard.NotNull(property, nameof(property));

                this.changed = changed;

                Binding binding = new Binding {  Source = source, Path = property, Mode = BindingMode.OneWay, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged };

                BindingOperations.SetBinding(this, ListenerProperty, binding);
            }

            private static void OnSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
            {
                ((DependencyPropertyListener)d).OnSourceChanged();
            }

            private void OnSourceChanged()
            {
                changed();
            }
        }
    }
}
