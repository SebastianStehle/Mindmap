// ==========================================================================
// LoadingManager.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using GP.Windows.Annotations;

namespace GP.Windows.Mvvm
{
    /// <summary>
    /// Notifies the system that some operation is loading right now.
    /// </summary>
    public sealed class LoadingManager : ILoadingManager
    {
        private readonly DispatcherTimer lazyTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(500) };
        private bool isLoading;

        /// <summary>
        /// Invoked, when a property has been changed.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Gets a value indicating if there is a operation running.
        /// </summary>
        public bool IsLoading
        {
            get { return isLoading; }
            set
            {
                if (isLoading != value)
                {
                    isLoading = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LoadingManager"/> class.
        /// </summary>
        public LoadingManager()
        {
            lazyTimer.Tick += LazyTimer_Tick;
        }

        private void LazyTimer_Tick(object sender, object e)
        {
            lazyTimer.Stop();

            IsLoading = false;
        }

        /// <summary>
        /// Starts the loading.
        /// </summary>
        public void BeginLoading()
        {
            IsLoading = true;
        }

        /// <summary>
        /// Finishs the loading.
        /// </summary>
        public void FinishLoading()
        {
            lazyTimer.Start();
        }

        /// <summary>
        /// Invokes the specified action when there is no loading operation.
        /// </summary>
        /// <param name="action">The action to invoke. Cannot be null</param>
        /// <exception cref="ArgumentNullException"><paramref name="action"/> is null.</exception>
        /// <returns>
        /// The task for synchronization.
        /// </returns>
        public async Task DoWhenNotLoadingAsync(Func<Task> action)
        {
            if (!IsLoading)
            {
                BeginLoading();
                try
                {
                    await action();
                }
                finally
                {
                    FinishLoading();
                }
            }
        }

        [NotifyPropertyChangedInvocator]
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
