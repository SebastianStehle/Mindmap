// ==========================================================================
// IExporter.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using GP.Utils;
using Hercules.Model.Rendering;

namespace Hercules.Model.ExImport
{
    public interface IExporter
    {
        string NameKey { get; }

        IEnumerable<FileExtension> Extensions { get; }

        Task ExportAsync(Document document, IRenderer renderer, Stream stream, PropertiesBag properties = null);
    }
}
