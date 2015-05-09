// ==========================================================================
// IOutlineGenerator.cs
// RavenMind Application
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

namespace RavenMind.Model.Export
{
    public interface IOutlineGenerator
    {
        string GenerateOutline(Document document, bool useColors, string noTextPlaceholder);
    }
}
