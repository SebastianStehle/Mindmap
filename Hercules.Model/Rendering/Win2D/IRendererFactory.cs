// ==========================================================================
// IRendererFactory.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================
using GP.Windows.UI.Controls;

namespace Hercules.Model.Rendering.Win2D
{
    public interface IRendererFactory
    {
        Win2DRenderer Current { get; }

        Win2DRenderer CreateRenderer(Document document, ICanvasControl canvas);
    }
}
