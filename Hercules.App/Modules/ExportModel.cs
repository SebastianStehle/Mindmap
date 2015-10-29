// ==========================================================================
// ExportModel.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using Hercules.Model.ExImport;

namespace Hercules.App.Modules
{
    public sealed class ExportModel
    {
        public IExportTarget Target { get; set; }

        public IExporter Exporter { get; set; }
    }
}
