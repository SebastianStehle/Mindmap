// ==========================================================================
// IRenderScene.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================
namespace Hercules.Model2.Rendering
{
    public interface IRenderScene
    {
        IRenderNode FindRenderNode(Node node);
    }
}
