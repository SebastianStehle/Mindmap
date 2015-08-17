// ==========================================================================
// DefaultRendererFactory.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================
using Microsoft.Graphics.Canvas.UI.Xaml;

namespace Hercules.Model.Rendering.Win2D.Default
{
    public class DefaultRendererFactory : IRendererFactory
    {
        public Win2DRenderer CreateRenderer(Document document, CanvasControl canvas)
        {
            return new DefaultRenderer(document, canvas);
        }
    }
}
