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
// ReSharper disable UnusedMemberInSuper.Global

namespace Hercules.Model2.Rendering
{
    public interface IRenderer
    {
        Task RenderScreenshotAsync(Stream stream, Vector3 background, float? dpi = null, float padding = 20);

        IRenderColor FindColor(Node node);

        IRenderIcon FindIcon(Node node);

        IAdornerRenderNode CreateAdorner(IRenderNode renderNode);

        void RemoveAdorner(IAdornerRenderNode adorner);

        void ShowPreviewElement(Vector2 position, Node parent, NodeSide anchor);

        void HidePreviewElement();

        void Invalidate();
    }
}
