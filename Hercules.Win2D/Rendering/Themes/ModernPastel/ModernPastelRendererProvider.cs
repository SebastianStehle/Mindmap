// ==========================================================================
// ModernPastelRendererProvider.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using GP.Windows.UI.Controls;
using Hercules.Model;

namespace Hercules.Win2D.Rendering.Themes.ModernPastel
{
    public class ModernPastelRendererProvider : IWin2DRendererProvider
    {
        public Win2DRenderer Current { get; set;  }

        public Win2DRenderer CreateRenderer(Document document, ICanvasControl canvas)
        {
            Current = new ModernPastelRenderer(document, canvas);

            return Current;
        }
    }
}
