// ==========================================================================
// ILoadingManager.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System;
using System.ComponentModel;
using System.Threading.Tasks;

namespace GP.Windows.Mvvm
{
    /// <summary>
    /// Notifies the system that some operation is loading right now.
    /// </summary>
    public interface ILoadingManager : INotifyPropertyChanged
    {
        /// <summary>
        /// Gets a value indicating if there is a operation running.
        /// </summary>
        bool IsLoading { get; }

        /// <summary>
        /// Starts the loading.
        /// </summary>
        void BeginLoading();

        /// <summary>
        /// Finishs the loading.
        /// </summary>
        void FinishLoading();

        /// <summary>
        /// Invokes the specified action when there is no loading operation.
        /// </summary>
        /// <param name="action">The action to invoke. Cannot be null</param>
        /// <exception cref="ArgumentNullException"><paramref name="action"/> is null.</exception>
        /// <returns>
        /// The task for synchronization.
        /// </returns>
        Task DoWhenNotLoadingAsync(Func<Task> action);
    }
}
