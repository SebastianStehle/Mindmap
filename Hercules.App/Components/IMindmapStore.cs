﻿// ==========================================================================
// IMindmapStore.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Windows.Storage;
using Hercules.Model;

namespace Hercules.App.Components
{
    public interface IMindmapStore
    {
        event EventHandler<DocumentFileEventArgs> FileLoaded;

        ObservableCollection<IDocumentFileModel> AllFiles { get; }

        IDocumentFileModel SelectedFile { get; }

        Task AddAsync(string name, Document document = null);

        Task LoadRecentsAsync();

        Task OpenAsync(StorageFile file);

        Task OpenAsync(IDocumentFileModel file);

        Task OpenRecentAsync();

        Task OpenFromFileAsync();

        Task SaveAsAsync();

        Task SaveAsync(bool hideDialogs = false);

        Task RemoveAsync(IDocumentFileModel file);

        Task RenameAsync(IDocumentFileModel file, string newName);
    }
}
