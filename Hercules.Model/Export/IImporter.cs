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

namespace Hercules.Model.Export
{
    public interface IImporter
    {
        Task<List<KeyValuePair<string, Document>>> ImportAsync(Stream stream, PropertiesBag properties = null);
    }
}
