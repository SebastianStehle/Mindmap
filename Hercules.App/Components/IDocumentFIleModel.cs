// ==========================================================================
// IDocumentFIleModel.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System.ComponentModel;
using Hercules.Model;

namespace Hercules.App.Components
{
    public interface IDocumentFileModel : INotifyPropertyChanged
    {
        string ModifiedLocal { get; }

        string DisplayPath { get; }

        string Name { get; }

        string Path { get; }

        bool HasChanges { get; }

        Document Document { get; }
    }
}
