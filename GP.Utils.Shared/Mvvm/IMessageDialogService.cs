// ==========================================================================
// IMessageDialogService.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System;
using System.IO;
using System.Threading.Tasks;

namespace GP.Utils.Mvvm
{
    /// <summary>
    /// MVVM service to show dialogs.
    /// </summary>
    public interface IMessageDialogService
    {
        /// <summary>
        /// Opens a dialog to save a file.
        /// </summary>
        /// <param name="extensions">The file extensions.</param>
        /// <param name="save">The action to write the file.</param>
        /// <exception cref="ArgumentNullException"><paramref name="extensions"/> is null.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="save"/> is null.</exception>
        /// <returns>
        /// The task for synchronization.
        /// </returns>
        Task SaveFileDialogAsync(string[] extensions, Func<Stream, Task> save);

        /// <summary>
        /// Opens a dialog to open a file.
        /// </summary>
        /// <param name="extensions">The file extensions.</param>
        /// <param name="open">The action to read the file.</param>
        /// <exception cref="ArgumentNullException"><paramref name="extensions"/> is null.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="open"/> is null.</exception>
        /// <returns>
        /// The task for synchronization.
        /// </returns>
        Task OpenFileDialogAsync(string[] extensions, Func<string, Stream, Task> open);

        /// <summary>
        /// Shows an confirm dialog.
        /// </summary>
        /// <param name="content">The content to show. Cannot be null or empty.</param>
        /// <exception cref="ArgumentNullException"><paramref name="content"/> is null.</exception>
        /// <exception cref="ArgumentException"><paramref name="content"/> is empty or contains only whitespaces.</exception>
        /// <returns>
        /// The task for synchronization with a boolean indicating if the user pressed OK.
        /// </returns>
        Task<bool> ConfirmAsync(string content);

        /// <summary>
        /// Shows an confirm dialog with the optional title.
        /// </summary>
        /// <param name="content">The content to show. Cannot be null or empty.</param>
        /// <param name="title">The optional title.</param>
        /// <exception cref="ArgumentNullException"><paramref name="content"/> is null.</exception>
        /// <exception cref="ArgumentException"><paramref name="content"/> is empty or contains only whitespaces.</exception>
        /// <returns>
        /// The task for synchronization with a boolean indicating if the user pressed OK.
        /// </returns>
        Task<bool> ConfirmAsync(string content, string title);

        /// <summary>
        /// Shows an alert dialog.
        /// </summary>
        /// <param name="content">The content to show. Cannot be null or empty.</param>
        /// <exception cref="ArgumentNullException"><paramref name="content"/> is null.</exception>
        /// <exception cref="ArgumentException"><paramref name="content"/> is empty or contains only whitespaces.</exception>
        /// <returns>
        /// The task for synchronization.
        /// </returns>
        Task AlertAsync(string content);

        /// <summary>
        /// Shows an alert dialog with the optional title.
        /// </summary>
        /// <param name="content">The content to show. Cannot be null or empty.</param>
        /// <param name="title">The optional title.</param>
        /// <exception cref="ArgumentNullException"><paramref name="content"/> is null.</exception>
        /// <exception cref="ArgumentException"><paramref name="content"/> is empty or contains only whitespaces.</exception>
        /// <returns>
        /// The task for synchronization.
        /// </returns>
        Task AlertAsync(string content, string title);
    }
}
