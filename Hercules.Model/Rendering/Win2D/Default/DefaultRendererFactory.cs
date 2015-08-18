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
        public Win2DRenderer Current { get; set;  }

        public Win2DRenderer CreateRenderer(Document document, ICanvasControl canvas)
        {
            Current =  new DefaultRenderer(document, canvas);

            return Current;
        }
    }
}
