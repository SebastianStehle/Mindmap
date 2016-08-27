// ==========================================================================
// IRenderer.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System.IO;
using System.Numerics;
using System.Threading.Tasks;

namespace Hercules.Model.Rendering
{
    public interface IRenderer
    {
        Task RenderScreenshotAsync(Stream stream, Vector3 background, float? dpi = null, float padding = 20);

        IRenderColor FindColor(NodeBase node);

        IRenderIcon FindIcon(NodeBase node);

        IAdornerRenderNode CreateAdorner(IRenderNode renderNode);

        void RemoveAdorner(IAdornerRenderNode adorner);

        void ShowPreviewElement(Vector2 position, NodeBase parent, NodeSide anchor);

        void HidePreviewElement();

        void Invalidate();
    }
}
