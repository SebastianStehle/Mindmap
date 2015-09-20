// ==========================================================================
// IMindmapRef.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System;
using System.Threading.Tasks;

namespace Hercules.App.Components
{
    public interface IMindmapRef
    {
        string Title { get; }

        DateTimeOffset LastUpdate { get; }

        Task RenameAsync(string newTitle);
    }
}