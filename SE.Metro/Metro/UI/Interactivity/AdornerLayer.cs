// ==========================================================================
// AdornerLayer.cs
// SE Requirements Application
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ===========================================================================

using System;
using System.Collections.ObjectModel;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace SE.Metro.UI.Interactivity
{
    /// <summary>
    /// This is an adorner layer and tries to work like the adorner layer 
    /// in windows presentation foundation (wpf). 
    /// </summary>
    /// <remarks>
    ///     <para>
    ///     The adorner layer is constructed at the top of the adorned object dynamically.
    ///     Therefore it has a priority list, where the layer will be 
    ///     created for a specific object:
    ///     <list type="number">
    ///         <item>
    ///             <description>The visual tree will be moved up. When one of the parents
    ///             is an adorner layer, this layer instance is used for the current object.</description>
    ///         </item>
    ///         <item>
    ///             <description>When no adorner layer or panel can be found in the visual tree, the method
    ///             to create a layer looks for a the next panel in the visual tree, when moving up. 
    ///             If such a panel could be found, the following will be done: 
    ///             First, when the item collection contains an adorner layer, use this, 
    ///             when not, create a new layer and add this layer to the panel.</description>
    ///         </item>
    ///     </list>
    ///     </para>
    /// </remarks>
    [TemplatePart(Name = ItemsControlPart, Type = typeof(ItemsControl))]
    public sealed class AdornerLayer : ContentControl
    {
        #region Constants

        /// <summary>
        /// Defines the name of the 'ItemsControl' template part, which works as a container 
        /// for all adorners in the this layer.
        /// </summary>
        public const string ItemsControlPart = "ItemsControl";

        #endregion

        #region Fields

        private readonly ObservableCollection<FrameworkElement> adornerContainers = new ObservableCollection<FrameworkElement>();

        #endregion

        #region Properties

        private readonly ObservableCollection<Adorner> adornerCollection = new ObservableCollection<Adorner>();
        private ReadOnlyObservableCollection<Adorner> adorners;
        /// <summary>
        /// Gets a readonly collection of all adorners that are part of this layer.
        /// </summary>
        /// <value>The adorners that are part of this layer. Will never be null.</value>
        public ReadOnlyObservableCollection<Adorner> Adorners
        {
            get
            {
                if (adorners == null)
                {
                    adorners = new ReadOnlyObservableCollection<Adorner>(adornerCollection);
                }

                return adorners;
            }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="AdornerLayer"/> class.
        /// </summary>
        public AdornerLayer()
        {
            DefaultStyleKey = typeof(AdornerLayer);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Invoked whenever application code or internal processes (such as a rebuilding
        /// layout pass) call ApplyTemplate. In simplest terms, this means the method
        /// is called just before a UI element displays in your app. Override this method
        /// to influence the default post-template logic of a class.
        /// </summary>
        protected override void OnApplyTemplate()
        {
            ItemsControl itemsControl = GetTemplateChild(ItemsControlPart) as ItemsControl;

            if (itemsControl != null)
            {
                itemsControl.ItemsSource = adornerContainers;
            }

            base.OnApplyTemplate();
        }

        /// <summary>
        /// Adds the specified adorner to the adorner layer.
        /// </summary>
        /// <param name="adorner">The adorner to add to the layer. Cannot be null.</param>
        /// <exception cref="ArgumentNullException"><paramref name="adorner"/> is null.</exception>
        internal void Add(Adorner adorner)
        {
            if (adorner == null)
            {
                throw new ArgumentNullException("adorner");
            }

            ContentControl contentControl = new ContentControl();
            contentControl.Content = adorner;
            contentControl.HorizontalContentAlignment = HorizontalAlignment.Stretch;
            contentControl.VerticalContentAlignment = VerticalAlignment.Stretch;

            adorner.Container = contentControl;

            adornerContainers.Add(contentControl);
            adornerCollection.Add(adorner);
        }

        /// <summary>
        /// Remove the specified adorner from the adorner layer.
        /// </summary>
        /// <param name="adorner">The adorner to remove from the layer. Cannot be null.</param>
        /// <exception cref="ArgumentNullException"><paramref name="adorner"/> is null.</exception>
        internal void Remove(Adorner adorner)
        {
            if (adorner == null)
            {
                throw new ArgumentNullException("adorner");
            }

            ContentControl container = adorner.Container;

            if (container != null)
            {
                container.Content = null;

                adorner.Container = null;

                adornerContainers.Remove(container);
                adornerCollection.Remove(adorner);
            }
        }

        /// <summary>
        /// Gets the <see cref="AdornerLayer"/> for the specified dependency object.
        /// </summary>
        /// <param name="dependencyObject">The dependency object to get the adorner layer for. Cannot be null.</param>
        /// <returns>The <see cref="AdornerLayer"/> for the specified dependency object.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="dependencyObject"/> is null.</exception>
        public static AdornerLayer GetAdornerLayer(DependencyObject dependencyObject)
        {
            if (dependencyObject == null)
            {
                throw new ArgumentNullException("dependencyObject");
            }

            AdornerLayer result = VisualTreeExtensions.FindParent<AdornerLayer>(dependencyObject);

            if (result == null)
            {
                Panel panel = VisualTreeExtensions.FindParent<Panel>(dependencyObject, x => x is Grid || x is Canvas);
                
                if (panel != null)
                {
                    result = GetAdornerLayerFromPanel(panel);
                }
            }

            return result;
        }

        private static AdornerLayer GetAdornerLayerFromPanel(Panel panel)
        {
            AdornerLayer result = panel.Children.OfType<AdornerLayer>().FirstOrDefault();

            if (result == null)
            {
                result = new AdornerLayer();

                result.SetValue(Canvas.ZIndexProperty, 5000);

                panel.Children.Add(result);
            }

            return result;
        }

        #endregion
    }
}
