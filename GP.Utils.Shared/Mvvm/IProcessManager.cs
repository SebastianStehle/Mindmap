// ==========================================================================
// IProcessManager.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System;
using System.ComponentModel;
using System.Threading.Tasks;

namespace GP.Utils.Mvvm
{
    /// <summary>
    /// Notifies the system that some operation is loading right now.
    /// </summary>
    public interface IProcessManager : INotifyPropertyChanged
    {
        /// <summary>
        /// Gets a value indicating if there is a operation running.
        /// </summary>
        bool IsMainProcessRunning { get; }

        /// <summary>
        /// Invokes the specified action when there is no loading operation.
        /// </summary>
        /// <param name="owner">The owner of the current process.</param>
        /// <param name="action">The action to invoke. Cannot be null</param>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="owner"/> is null.
        ///     <paramref name="action"/> is null.
        /// </exception>
        /// <returns>
        /// The task for synchronization.
        /// </returns>
        Task RunMainProcessAsync(object owner, Func<Task> action);
    }
}
