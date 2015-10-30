// ==========================================================================
// IRenderer.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System.Threading.Tasks;
using Windows.Graphics.Printing;
using Windows.Storage.Streams;
using Windows.UI;

namespace Hercules.Model.Rendering
{
    public interface IRenderer
    {
        Task RenderScreenshotAsync(IRandomAccessStream stream, Color background, float? dpi = null, float padding = 20);

        LayoutThemeColor FindColor(NodeBase node);

        IPrintDocumentSource Print(float padding = 20);
    }
}
