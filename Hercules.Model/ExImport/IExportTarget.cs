// ==========================================================================
// IExportTarget.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System.Threading.Tasks;
using Hercules.Model.Rendering;

namespace Hercules.Model.ExImport
{
    public interface IExportTarget
    {
        string NameKey { get; }

        Task ExportAsync(string name, Document document, IExporter exporter, IRenderer renderer);
    }
}
