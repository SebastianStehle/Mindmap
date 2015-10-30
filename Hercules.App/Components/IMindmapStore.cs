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
using Hercules.Model;

namespace Hercules.App.Components
{
    public interface IMindmapStore
    {
        event EventHandler<DocumentLoadedEventArgs> DocumentLoaded;

        ObservableCollection<IMindmapRef> AllMindmaps { get; }

        Document LoadedDocument { get; }

        Task SaveAsync();

        Task LoadAsync(IMindmapRef mindmap);

        Task LoadAllAsync();

        Task DeleteAsync(IMindmapRef mindmap);

        Task CreateAsync(string name);

        Task AddNewNonLoadingAsync(string name, Document document);
    }
}
