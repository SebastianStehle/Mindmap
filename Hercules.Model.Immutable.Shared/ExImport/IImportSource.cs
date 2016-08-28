// ==========================================================================
// IImportSource.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System.Collections.Generic;
using System.Threading.Tasks;

namespace Hercules.Model.ExImport
{
    public interface IImportSource
    {
        string NameKey { get; }

        Task<List<ImportResult>> ImportAsync(IImporter importer);
    }
}
