// ==========================================================================
// IOutlineGenerator.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================
namespace Hercules.Model.Export
{
    public interface IOutlineGenerator
    {
        string GenerateOutline(Document document, bool useColors, string noTextPlaceholder);
    }
}
