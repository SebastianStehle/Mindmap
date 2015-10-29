// ==========================================================================
// IExportTarget.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System.Threading.Tasks;
using Hercules.Model;
using Hercules.Model.Export;
using Hercules.Model.Rendering;

namespace Hercules.App.Components
{
    public interface IExportTarget
    {
        string NameKey { get; }

        Task ExportAsync(Document document, IExporter exporter, IRenderer renderer);
    }
}
