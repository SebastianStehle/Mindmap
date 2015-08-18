// ==========================================================================
// DefaultRendererFactory.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================
using GP.Windows.UI.Controls;

namespace Hercules.Model.Rendering.Win2D.Default
{
    public class DefaultRendererFactory : IRendererFactory
    {
        public Win2DRenderer CreateRenderer(Document document, ICanvasControl canvas)
        {
            return new DefaultRenderer(document, canvas);
        }
    }
}
