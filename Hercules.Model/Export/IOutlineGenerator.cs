// ==========================================================================
// IOutlineGenerator.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System.IO;
using Hercules.Model.Layouting;

namespace Hercules.Model.Export
{
    public interface IOutlineGenerator
    {
        void GenerateOutline(Document document, IRenderer renderer, Stream stream, bool useColors, string noTextPlaceholder);

        string GenerateOutlineToString(Document document, IRenderer renderer, bool useColors, string noTextPlaceholder);
    }
}
