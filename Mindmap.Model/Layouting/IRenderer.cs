// ==========================================================================
// IRenderer.cs
// Mindmap Application
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

namespace MindmapApp.Model.Layouting
{
    public interface IRenderer
    {
        IRenderNode FindRenderNode(NodeBase node);
    }
}
