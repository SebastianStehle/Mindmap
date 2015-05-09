﻿// ==========================================================================
// LoadableControl.cs
// Metro Library SE
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace SE.Metro.UI.Controls
{
    /// <summary>
    /// Base class to handle loaded and unloaded events in an easier way.
    /// </summary>
    public abstract class LoadableControl : Control
    {
        #region Properties

        /// <summary>
        /// Gets a value indicating if this control is loaded and part of the visual tree.
        /// </summary>
        /// <value>A value indicating if this control is loaded and therefore part of the visual tree.</value>
        public bool IsLoaded { get; private set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="LoadableControl"/> class
        /// /// </summary>
        protected LoadableControl()
        {
            Loaded += LoadableControl_Loaded;

            Unloaded += LoadableControl_Unloaded;
        }

        #endregion

        #region Methods

        private void LoadableControl_Loaded(object sender, RoutedEventArgs e)
        {
            IsLoaded = true;

            OnLoaded();
        }

        private void LoadableControl_Unloaded(object sender, RoutedEventArgs e)
        {
            IsLoaded = false;

            OnUnloaded();
        }

        /// <summary>
        /// Occurs after the control has been loaded.
        /// </summary>
        protected virtual void OnLoaded()
        {
        }

        /// <summary>
        /// Occurs after the control has been unloaded.
        /// </summary>
        protected virtual void OnUnloaded()
        {
        }

        #endregion
    }
}
