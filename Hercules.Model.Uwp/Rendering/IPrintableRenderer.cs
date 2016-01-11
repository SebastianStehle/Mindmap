// ==========================================================================
// IRenderer.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using Windows.Graphics.Printing;

namespace Hercules.Model.Rendering
{
    public interface IPrintableRenderer : IRenderer
    {
        IPrintDocumentSource Print(float padding = 20);
    }
}
