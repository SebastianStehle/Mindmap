// ==========================================================================
// ExportModel.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using Hercules.App.Components;
using Hercules.Model.Export;

namespace Hercules.App.Modules
{
    public sealed class ExportModel
    {
        public IExportTarget Target { get; set; }

        public IExporter Exporter { get; set; }
    }
}
