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
using Windows.Storage.Streams;

namespace Hercules.App.Components
{
    public interface IMessageDialogService
    {
        Task SaveFileDialogAsync(string[] extensions, Action<Stream> save);

        Task SaveFileDialogAsync(string[] extensions, Func<Stream, Task> save);

        Task SaveFileDialogAsync(string[] extensions, Action<IRandomAccessStream> save);

        Task SaveFileDialogAsync(string[] extensions, Func<IRandomAccessStream, Task> save);

        Task OpenFileDialogAsync(string[] extensions, Action<Stream> open);

        Task OpenFileDialogAsync(string[] extensions, Func<Stream, Task> open);

        Task OpenFileDialogAsync(string[] extensions, Action<IRandomAccessStream> open);

        Task OpenFileDialogAsync(string[] extensions, Func<IRandomAccessStream, Task> open);

        Task<bool> ConfirmAsync(string content);

        Task<bool> ConfirmAsync(string content, string title);

        Task AlertAsync(string content);

        Task AlertAsync(string content, string title);
    }
}
