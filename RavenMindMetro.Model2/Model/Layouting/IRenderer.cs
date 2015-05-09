// ==========================================================================
// IRenderEngine.cs
// RavenMind Application
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

namespace RavenMind.Model.Layouting
{
    public interface IRenderer
    {
        IRenderNode FindRenderNode(NodeBase node);
    }
}
