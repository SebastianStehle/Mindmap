// ==========================================================================
// IExporter.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System.IO;
using System.Threading.Tasks;
using Hercules.Model.Rendering;

namespace Hercules.Model.Export
{
    public interface IExporter
    {
        Task ExportAsync(Document document, IRenderer renderer, Stream stream, PropertiesBag properties = null);
    }
}
