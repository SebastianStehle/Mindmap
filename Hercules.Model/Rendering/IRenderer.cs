// ==========================================================================
// IRenderer.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System.Numerics;
using System.Threading.Tasks;
using Windows.Graphics.Printing;
using Windows.Storage.Streams;
using Windows.UI;
using Hercules.Model.Layouting;

namespace Hercules.Model.Rendering
{
    public interface IRenderer
    {
        Task RenderScreenshotAsync(IRandomAccessStream stream, Color background, float? dpi = null, float padding = 20);

        IRenderColor FindColor(NodeBase node);

        IRenderIcon FindIcon(NodeBase node);

        IPrintDocumentSource Print(float padding = 20);

        IAdornerRenderNode CreateAdorner(IRenderNode renderNode);

        void RemoveAdorner(IAdornerRenderNode adorner);

        void ShowPreviewElement(Vector2 position, NodeBase parent, AnchorPoint anchor);

        void HidePreviewElement();

        void Invalidate();
    }
}
