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
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.UI.Popups;
using GP.Windows;
using Hercules.Model.Utils;

namespace Hercules.App.Components.Implementations
{
    public sealed class MessageDialogService : IMessageDialogService
    {
        public async Task OpenFileDialogAsync(string[] extensions, Action<Stream> open)
        {
            Guard.NotNull(extensions, nameof(extensions));
            Guard.NotNull(open, nameof(open));

            FileOpenPicker filePicker = CreateFileOpenPicker(extensions);

            StorageFile file = await filePicker.PickSingleFileAsync();

            if (file != null)
            {
                using (Stream fileStream = await file.OpenStreamForWriteAsync())
                {
                    open(fileStream);
                }
            }
        }

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

        public async Task OpenFileDialogAsync(string[] extensions, Action<IRandomAccessStream> open)
        {
            Guard.NotNull(extensions, nameof(extensions));
            Guard.NotNull(open, nameof(open));

            FileOpenPicker filePicker = CreateFileOpenPicker(extensions);

            StorageFile file = await filePicker.PickSingleFileAsync();

            if (file != null)
            {
                using (IRandomAccessStream fileStream = await file.OpenAsync(FileAccessMode.ReadWrite))
                {
                    open(fileStream);
                }
            }
        }

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

        public async Task SaveFileDialogAsync(string[] extensions, Action<Stream> save)
        {
            Guard.NotNull(extensions, nameof(extensions));
            Guard.NotNull(save, nameof(save));

            FileSavePicker filePicker = CreateFileSavePicker(extensions);

            StorageFile file = await filePicker.PickSaveFileAsync();

            if (file != null)
            {
                using (Stream fileStream = await file.OpenStreamForWriteAsync())
                {
                    save(fileStream);
                }
            }
        }

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

        public async Task SaveFileDialogAsync(string[] extensions, Action<IRandomAccessStream> save)
        {
            Guard.NotNull(extensions, nameof(extensions));
            Guard.NotNull(save, nameof(save));

            FileSavePicker filePicker = CreateFileSavePicker(extensions);

            StorageFile file = await filePicker.PickSaveFileAsync();

            if (file != null)
            {
                using (IRandomAccessStream fileStream = await file.OpenAsync(FileAccessMode.ReadWrite))
                {
                    save(fileStream);
                }
            }
        }

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

        public Task AlertAsync(string content)
        {
            return AlertAsync(content, null);
        }

        public async Task AlertAsync(string content, string title)
        {
            Guard.NotNullOrEmpty(content, nameof(content));

            MessageDialog dialog = new MessageDialog(content, title);

            dialog.Commands.Add(new UICommand(ResourceManager.GetString("Common_OK")));

            dialog.CancelCommandIndex = 0;
            dialog.DefaultCommandIndex = 0;

            await dialog.ShowAsync();
        }

        public Task<bool> ConfirmAsync(string content)
        {
            return ConfirmAsync(content, null);
        }

        public async Task<bool> ConfirmAsync(string content, string title)
        {
            Guard.NotNullOrEmpty(content, nameof(content));

            MessageDialog dialog = new MessageDialog(content, title);

            TaskCompletionSource<bool> completionSource = new TaskCompletionSource<bool>();

            dialog.Commands.Add(new UICommand(
                ResourceManager.GetString("Common_Yes"), x =>
                {
                    completionSource.SetResult(true);
                }));
            dialog.Commands.Add(new UICommand(
                ResourceManager.GetString("Common_No"), x =>
                {
                    completionSource.SetResult(false);
                }));

            dialog.CancelCommandIndex = 1;
            dialog.DefaultCommandIndex = 0;

            await dialog.ShowAsync();

            return await completionSource.Task;
        }
    }
}
