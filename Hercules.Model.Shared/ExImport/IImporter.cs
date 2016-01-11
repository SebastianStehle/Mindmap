// ==========================================================================
// IImporter.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using GP.Utils;

namespace Hercules.Model.ExImport
{
    public interface IImporter
    {
        string NameKey { get; }

        IEnumerable<FileExtension> Extensions { get; }

        Task<List<ImportResult>> ImportAsync(Stream stream, string name, PropertiesBag properties = null);
    }
}
