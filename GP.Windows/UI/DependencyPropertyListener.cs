// ==========================================================================
// DependencyPropertyListener.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace GP.Windows.UI
{
    internal sealed class DependencyPropertyListener : DependencyObject
    {
        private readonly Action changed;

        private static readonly DependencyProperty ListenerProperty =
            DependencyProperty.Register("Listener", typeof(object), typeof(DependencyPropertyListener), new PropertyMetadata(null, OnSourceChanged));

        public DependencyPropertyListener(DependencyObject source, string property, Action changed)
            : this(source, new PropertyPath(property), changed)
        {
        }

        private DependencyPropertyListener(DependencyObject source, PropertyPath property, Action changed)
        {
            Guard.NotNull(changed, nameof(changed));
            Guard.NotNull(source, nameof(source));
            Guard.NotNull(property, nameof(property));

            this.changed = changed;

            Binding binding = new Binding { Source = source, Path = property, Mode = BindingMode.OneWay, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged };

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
