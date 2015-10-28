// ==========================================================================
// IOutlineGenerator.cs
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
    public interface IOutlineGenerator
    {
        Task WriteOutlineAsync(Document document, IRenderer renderer, Stream stream, bool useColors, string noTextPlaceholder);
    }
}
