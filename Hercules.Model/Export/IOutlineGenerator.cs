// ==========================================================================
// IOutlineGenerator.cs
// Hercules Application
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
