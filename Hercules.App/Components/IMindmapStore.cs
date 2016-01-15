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
using Hercules.Model.Storing;

namespace Hercules.App.Components
{
    public interface IMindmapStore
    {
        event EventHandler<DocumentFileEventArgs> DocumentLoaded;

        ObservableCollection<DocumentFile> AllDocuments { get; }

        DocumentFile LoadedDocument { get; }

        Task LoadRecentAsync();

        Task CreateAsync();

        Task OpenAsync(DocumentFile file);

        Task OpenAsync();

        Task SaveAsync();

        Task SaveToFileAsync();
    }
}
