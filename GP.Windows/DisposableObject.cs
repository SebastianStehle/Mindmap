// ==========================================================================
// DisposableObject.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================
using System;

namespace GP.Windows
{
    /// <summary>
    /// Base class for all disposable objects.
    /// </summary>
    public abstract class DisposableObject : IDisposable
    {
        private readonly object disposeLock = new object();
        private bool isDisposed;

        /// <summary>
        /// Gets a value indicating whether the current instance is disposed.
        /// </summary>
        /// <value>
        /// <c>true</c> if the current instance is disposed; otherwise, <c>false</c>.
        /// </value>
        public bool IsDisposed
        {
            get
            {
                return isDisposed;
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Tries the dispose the current instance.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; 
        /// <c>false</c> to release only unmanaged resources.</param>
        protected void Dispose(bool disposing)
        {
            lock (disposeLock)
            {
                if (!isDisposed)
                {
                    DisposeObject(disposing);

                    isDisposed = true;
                }
            }
        }

        /// <summary>
        /// Releases unmanaged and optionally managed resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; 
        /// <c>false</c> to release only unmanaged resources.</param>
        protected abstract void DisposeObject(bool disposing);

        /// <summary>
        /// Initializes a new instance of the <see cref="DisposableObject"/> class.
        /// </summary>
        protected DisposableObject()
        {
        }

        /// <summary>
        /// Throws an exception if the current instance is disposed.
        /// </summary>
        /// <exception cref="System.ObjectDisposedException">The object has already been disposed.</exception>
        protected void ThrowIfDisposed()
        {
            if (isDisposed)
            {
                throw new ObjectDisposedException(GetType().Name);
            }
        }
    }
}
