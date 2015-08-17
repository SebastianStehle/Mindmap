// ==========================================================================
// IRendererFactory.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================
using Microsoft.Graphics.Canvas.UI.Xaml;

namespace Hercules.Model.Rendering.Win2D
{
    public interface IRendererFactory
    {
        Win2DRenderer CreateRenderer(Document document, CanvasControl canvas);
    }
}
