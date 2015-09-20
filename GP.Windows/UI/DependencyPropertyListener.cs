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
        {
            Guard.NotNull(source, nameof(source));
            Guard.NotNull(changed, nameof(changed));
            Guard.NotNullOrEmpty(property, nameof(property));

            this.changed = changed;
            
            BindingOperations.SetBinding(this, ListenerProperty,
                new Binding { Source = source, Path = new PropertyPath(property), UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged });
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
