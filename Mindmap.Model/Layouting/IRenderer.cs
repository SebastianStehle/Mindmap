// ==========================================================================
// IRenderer.cs
// Mindmap Application
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

namespace Mindmap.Model.Layouting
{
    public interface IRenderer
    {
        IRenderNode FindRenderNode(NodeBase node);
    }
}
