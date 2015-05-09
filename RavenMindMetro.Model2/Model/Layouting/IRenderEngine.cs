// ==========================================================================
// IRenderEngine.cs
// RavenMind Application
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

namespace RavenMind.Model.Layouting
{
    public interface IRenderEngine
    {
        IRenderNode FindRenderNode(NodeBase node);
    }
}
