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

namespace GP.Utils.UI
{
    internal sealed class DependencyPropertyListener : DependencyObject
    {
        private Action changedCallback;

        private static readonly DependencyProperty ListenerProperty =
            DependencyProperty.Register("Listener", typeof(object), typeof(DependencyPropertyListener), new PropertyMetadata(null, OnSourceChanged));

        private static void OnSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((DependencyPropertyListener)d).OnSourceChanged();
        }

        public DependencyPropertyListener(DependencyObject source, string property, Action changedCallback)
        {
            Guard.NotNull(source, nameof(source));
            Guard.NotNull(changedCallback, nameof(changedCallback));
            Guard.NotNullOrEmpty(property, nameof(property));

            this.changedCallback = changedCallback;

            Bind(source, property);
        }

        private void Bind(DependencyObject source, string property)
        {
            Binding binding = new Binding { Source = source, Path = new PropertyPath(property), UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged };

            BindingOperations.SetBinding(this, ListenerProperty, binding);
        }

        private void OnSourceChanged()
        {
            changedCallback();
        }

        public void Release()
        {
            changedCallback = null;
        }
    }
}
