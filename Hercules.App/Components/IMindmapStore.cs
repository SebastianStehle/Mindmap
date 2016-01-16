// ==========================================================================
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

        ObservableCollection<DocumentFileModel> AllFiles { get; }

        DocumentFileModel SelectedFile { get; }

        void Add(string name, Document document);

        Task LoadRecentsAsync();

        Task CreateAsync();

        Task OpenAsync(StorageFile file);

        Task OpenAsync(DocumentFileModel file);

        Task OpenRecentAsync();

        Task OpenFromFileAsync();

        Task SaveAsAsync();

        Task SaveAsync(bool hideDialogs = false);

        Task RemoveAsync(DocumentFileModel file);
    }
}
