// ==========================================================================
// IOutlineGenerator.cs
// Mindmap Application
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

namespace MindmapApp.Model.Export
{
    public interface IOutlineGenerator
    {
        string GenerateOutline(Document document, bool useColors, string noTextPlaceholder);
    }
}
