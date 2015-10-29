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
using GP.Windows;

namespace Hercules.Model.Export
{
    public interface IImporter
    {
        string NameKey { get; }

        IEnumerable<FileExtension> Extensions { get; }

        Task<List<KeyValuePair<string, Document>>> ImportAsync(Stream stream, PropertiesBag properties = null);
    }
}
