// ==========================================================================
// IRenderScene.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================
namespace Hercules.Model.Layouting
{
    public interface IRenderScene
    {
        IRenderNode FindRenderNode(NodeBase node);
    }
}
