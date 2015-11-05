// ==========================================================================
// MessageDialogService.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Windows.ApplicationModel.Resources;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.UI.Popups;

namespace GP.Windows.Mvvm
{
    /// <summary>
    /// MVVM service to show dialogs.
    /// </summary>
    public sealed class MessageDialogService : IMessageDialogService
    {
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
        public async Task OpenFileDialogAsync(string[] extensions, Func<Stream, Task> open)
        {
            Guard.NotNull(extensions, nameof(extensions));
            Guard.NotNull(open, nameof(open));

            FileOpenPicker filePicker = CreateFileOpenPicker(extensions);

            StorageFile file = await filePicker.PickSingleFileAsync();

            if (file != null)
            {
                using (Stream fileStream = await file.OpenStreamForWriteAsync())
                {
                    await open(fileStream);
                }
            }
        }

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
        public async Task OpenFileDialogAsync(string[] extensions, Func<IRandomAccessStream, Task> open)
        {
            Guard.NotNull(extensions, nameof(extensions));
            Guard.NotNull(open, nameof(open));

            FileOpenPicker filePicker = CreateFileOpenPicker(extensions);

            StorageFile file = await filePicker.PickSingleFileAsync();

            if (file != null)
            {
                using (IRandomAccessStream fileStream = await file.OpenAsync(FileAccessMode.ReadWrite))
                {
                    await open(fileStream);
                }
            }
        }

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
        public async Task SaveFileDialogAsync(string[] extensions, Func<Stream, Task> save)
        {
            Guard.NotNull(extensions, nameof(extensions));
            Guard.NotNull(save, nameof(save));

            FileSavePicker filePicker = CreateFileSavePicker(extensions);

            StorageFile file = await filePicker.PickSaveFileAsync();

            if (file != null)
            {
                using (Stream fileStream = await file.OpenStreamForWriteAsync())
                {
                    await save(fileStream);
                }
            }
        }

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
        public async Task SaveFileDialogAsync(string[] extensions, Func<IRandomAccessStream, Task> save)
        {
            Guard.NotNull(extensions, nameof(extensions));
            Guard.NotNull(save, nameof(save));

            FileSavePicker filePicker = CreateFileSavePicker(extensions);

            StorageFile file = await filePicker.PickSaveFileAsync();

            if (file != null)
            {
                using (IRandomAccessStream fileStream = await file.OpenAsync(FileAccessMode.ReadWrite))
                {
                    await save(fileStream);
                }
            }
        }

        private static FileOpenPicker CreateFileOpenPicker(string[] extensions)
        {
            FileOpenPicker filePicker = new FileOpenPicker();

            if (extensions != null)
            {
                foreach (string extension in extensions)
                {
                    filePicker.FileTypeFilter.Add(extension);
                }
            }

            return filePicker;
        }

        private static FileSavePicker CreateFileSavePicker(string[] extensions)
        {
            FileSavePicker filePicker = new FileSavePicker();

            if (extensions != null)
            {
                foreach (string extension in extensions)
                {
                    filePicker.FileTypeChoices.Add(extension, new List<string> { extension });
                }
            }

            return filePicker;
        }

        /// <summary>
        /// Shows an alert dialog.
        /// </summary>
        /// <param name="content">The content to show. Cannot be null or empty.</param>
        /// <exception cref="ArgumentNullException"><paramref name="content"/> is null.</exception>
        /// <exception cref="ArgumentException"><paramref name="content"/> is empty or contains only whitespaces.</exception>
        /// <returns>
        /// The task for synchronization.
        /// </returns>
        public Task AlertAsync(string content)
        {
            return AlertAsync(content, null);
        }

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
        public async Task AlertAsync(string content, string title)
        {
            Guard.NotNullOrEmpty(content, nameof(content));

            MessageDialog dialog = string.IsNullOrWhiteSpace(title) ? new MessageDialog(content) : new MessageDialog(content, title);

            dialog.Commands.Add(new UICommand(GetString("Common_OK")));

            dialog.CancelCommandIndex = 0;
            dialog.DefaultCommandIndex = 0;

            await dialog.ShowAsync();
        }

        /// <summary>
        /// Shows an confirm dialog.
        /// </summary>
        /// <param name="content">The content to show. Cannot be null or empty.</param>
        /// <exception cref="ArgumentNullException"><paramref name="content"/> is null.</exception>
        /// <exception cref="ArgumentException"><paramref name="content"/> is empty or contains only whitespaces.</exception>
        /// <returns>
        /// The task for synchronization with a boolean indicating if the user pressed OK.
        /// </returns>
        public Task<bool> ConfirmAsync(string content)
        {
            return ConfirmAsync(content, null);
        }

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
        public async Task<bool> ConfirmAsync(string content, string title)
        {
            Guard.NotNullOrEmpty(content, nameof(content));

            MessageDialog dialog = string.IsNullOrWhiteSpace(title) ? new MessageDialog(content) : new MessageDialog(content, title);

            TaskCompletionSource<bool> completionSource = new TaskCompletionSource<bool>();

            dialog.Commands.Add(new UICommand(
                GetString("Common_Yes"), x =>
                {
                    completionSource.SetResult(true);
                }));
            dialog.Commands.Add(new UICommand(
                GetString("Common_No"), x =>
                {
                    completionSource.SetResult(false);
                }));

            dialog.CancelCommandIndex = 1;
            dialog.DefaultCommandIndex = 0;

            await dialog.ShowAsync();

            return await completionSource.Task;
        }

        private static string GetString(string key)
        {
            ResourceLoader resourceLoader = new ResourceLoader();

            string result = resourceLoader.GetString(key);

            if (string.IsNullOrWhiteSpace(result))
            {
                throw new ArgumentException($"Cannot find text with key '{key}'.");
            }

            return result;
        }
    }
}
