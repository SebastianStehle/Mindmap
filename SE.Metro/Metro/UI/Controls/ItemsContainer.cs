// ==========================================================================
// ItemsContainer.cs
// Metro Library SE
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace SE.Metro.UI.Controls
{
    /// <summary>
    /// Base class to bind the items of an observable collection to controls without using item containers.
    /// </summary>
    /// <typeparam name="TItem">The type of the items.</typeparam>
    /// <typeparam name="TControl">The type of the controls.</typeparam>
    [TemplatePart(Name = PartPanel, Type = typeof(Panel))]
    public abstract class ItemsContainer<TItem, TControl> : LoadableControl where TControl : FrameworkElement, new()
    {
        #region Constants

        private const string PartPanel = "Panel";

        #endregion

        #region Fields
        
        private readonly Dictionary<TItem, TControl> controls = new Dictionary<TItem, TControl>();
        private readonly Dictionary<TItem, TControl> controlCache = new Dictionary<TItem, TControl>();
        private Panel controlsPanel;

        #endregion

        #region Dependency Properties
            
        /// <summary>
        /// Defines the <see cref="Items"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ItemsProperty =
            DependencyProperty.Register("Items", typeof(ObservableCollection<TItem>), typeof(ItemsContainer<TItem, TControl>), new PropertyMetadata(null, new PropertyChangedCallback(OnItemsChanged)));
        /// <summary>
        /// Gets or sets the items to bind.
        /// </summary>
        /// <value>The items to bind.</value>
        public ObservableCollection<TItem> Items
        {
            get { return (ObservableCollection<TItem>)GetValue(ItemsProperty); }
            set { SetValue(ItemsProperty, value); }
        }

        private static void OnItemsChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            var owner = o as ItemsContainer<TItem, TControl>;
            if (owner != null)
            {
                owner.OnItemsChanged(e);
            }
        }

        private void OnItemsChanged(DependencyPropertyChangedEventArgs e)
        {
            ObservableCollection<TItem> oldCollection = e.OldValue as ObservableCollection<TItem>;
            ObservableCollection<TItem> newCollection = e.NewValue as ObservableCollection<TItem>;

            UpdateCollectionBinding(oldCollection, newCollection);
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ItemsContainer&lt;TItem,TControl&gt;"/> class.
        /// </summary>
        protected ItemsContainer()
        {
        }

        #endregion

        #region Methods

        /// <summary>
        /// Occurs when the collection has been changed.
        /// </summary>
        protected virtual void OnCollectionChanged()
        {
        }

        /// <summary>
        /// Occurs when a new control has been added.
        /// </summary>
        /// <param name="item">The item for this control.</param>
        /// <param name="control">The added control.</param>
        protected virtual void OnControlAdded(TItem item, TControl control)
        {
        }

        /// <summary>
        /// Occurs when an existing control has been removed.
        /// </summary>
        /// <param name="item">The item for this control.</param>
        /// <param name="control">The removed control.</param>
        protected virtual void OnControlRemoved(TItem item, TControl control)
        {
        }

        private void HandleItemAdded(TItem item)
        {
            TControl control = controlCache.GetOrCreateDefault(item);
            control.DataContext = item;
            controls[item] = control;

            VisualTreeExtensions.TryAdd(controlsPanel, control);

            OnControlAdded(item, control);
        }

        private void HandleItemRemoved(TItem node)
        {
            TControl control = controlCache[node];
            control.DataContext = null;
            controls.Remove(node);

            VisualTreeExtensions.TryRemove(controlsPanel, control);

            OnControlRemoved(node, control);
        }

        private void UpdateCollectionBinding(ObservableCollection<TItem> oldCollection, ObservableCollection<TItem> newCollection)
        {
            if (oldCollection != null)
            {
                INotifyCollectionChanged collectionChanged = oldCollection;

                collectionChanged.CollectionChanged -= collection_CollectionChanged;

                foreach (TItem item in controls.Keys.ToList())
                {
                    HandleItemRemoved(item);
                }
            }

            if (newCollection != null)
            {
                controlCache.Clear();

                INotifyCollectionChanged collectionChanged = newCollection;

                collectionChanged.CollectionChanged += collection_CollectionChanged;

                foreach (TItem item in newCollection)
                {
                    HandleItemAdded(item);
                }
            }

            OnCollectionChanged();
        }

        private void collection_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Reset)
            {
                foreach (TItem item in controls.Keys.ToList())
                {
                    HandleItemRemoved(item);
                }
            }
            else
            {
                if (e.OldItems != null)
                {
                    foreach (TItem item in e.OldItems)
                    {
                        HandleItemRemoved(item);
                    }
                }

                if (e.NewItems != null)
                {
                    foreach (TItem item in e.NewItems)
                    {
                        HandleItemAdded(item);
                    }
                }
            }
        }

        /// <summary>
        /// Invoked whenever application code or internal processes (such as a rebuilding layout pass) call ApplyTemplate. 
        /// In simplest terms, this means the method  is called just before a UI element displays in your app. 
        /// Override this method to influence the default post-template logic of a class.
        /// </summary>
        protected override void OnApplyTemplate()
        {
            controlsPanel = (Panel)GetTemplateChild(PartPanel);

            if (controlsPanel != null)
            {
                foreach (TControl control in controls.Values)
                {
                    controlsPanel.Children.Add(control);
                }
            }
        }

        #endregion
    }
}
