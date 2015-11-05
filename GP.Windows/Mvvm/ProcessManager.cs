// ==========================================================================
// ProcessManager.cs
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

namespace GP.Windows.Mvvm
{
    /// <summary>
    /// Notifies the system that some operation is loading right now.
    /// </summary>
    public sealed class ProcessManager : IProcessManager
    {
        private readonly DispatcherTimer lazyTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(500) };
        private bool isMainProcessRunning;
        private object currentOwner;

        /// <summary>
        /// Invoked, when a property has been changed.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Gets a value indicating if there is a operation running.
        /// </summary>
        public bool IsMainProcessRunning
        {
            get
            {
                return isMainProcessRunning;
            }
            set
            {
                if (isMainProcessRunning != value)
                {
                    isMainProcessRunning = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProcessManager"/> class.
        /// </summary>
        public ProcessManager()
        {
            lazyTimer.Tick += LazyTimer_Tick;
        }

        private void LazyTimer_Tick(object sender, object e)
        {
            lazyTimer.Stop();

            IsMainProcessRunning = false;
        }

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
        public async Task RunMainProcessAsync(object owner, Func<Task> action)
        {
            Guard.NotNull(owner, nameof(owner));
            Guard.NotNull(action, nameof(action));

            if (!IsMainProcessRunning || currentOwner == owner)
            {
                currentOwner = owner;

                IsMainProcessRunning = true;
                try
                {
                    await action();
                }
                finally
                {
                    lazyTimer.Start();
                }
            }
        }

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
