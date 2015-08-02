// ==========================================================================
// IRenderer.cs
// Hercules Application
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

namespace Hercules.Model.Layouting
{
    public interface IRenderer
    {
        IRenderNode FindRenderNode(NodeBase node);
    }
}
