// ==========================================================================
// IImportSource.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System.Collections.Generic;
using System.Threading.Tasks;
using Hercules.Model;
using Hercules.Model.Export;

namespace Hercules.App.Components
{
    public interface IImportSource
    {
        string NameKey { get; }

        Task<List<KeyValuePair<string, Document>>> ImportAsync(IImporter importer);
    }
}
