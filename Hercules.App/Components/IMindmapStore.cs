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

        ObservableCollection<MindmapRef> AllMindmaps { get; }

        Document LoadedDocument { get; }

        MindmapRef LoadedMindmap { get; }

        bool IsValidMindmapName(string name);

        Task CreateAsync(string name);

        Task SaveAsync();

        Task SaveAsync(MindmapRef mindmap, Document document);

        Task LoadAsync(MindmapRef mindmap);

        Task LoadAllAsync();

        Task RenameAsync(MindmapRef mindmap, string newName);

        Task DeleteAsync(MindmapRef mindmap);

        Task AddAsync(string name, Document document);
    }
}
