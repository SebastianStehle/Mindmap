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

namespace Hercules.Model.Layouting
{
    public interface IRenderer
    {
        Task RenderScreenshotAsync(IRandomAccessStream stream, Color background, float dpi, float padding = 20);

        ThemeColor FindColor(NodeBase node);

        IPrintDocumentSource Print();

        IRenderNode FindRenderNode(NodeBase node);
    }
}
