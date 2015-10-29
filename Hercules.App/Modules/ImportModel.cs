// ==========================================================================
// ImportModel.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using Hercules.App.Components;
using Hercules.Model.Export;

namespace Hercules.App.Modules
{
    public sealed class ImportModel
    {
        public IImportSource Source { get; set; }

        public IImporter Importer { get; set; }
    }
}
