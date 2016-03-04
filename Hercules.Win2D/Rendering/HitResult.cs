// ==========================================================================
// HitResult.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using GP.Utils;

namespace Hercules.Win2D.Rendering
{
    public sealed class HitResult
    {
        private readonly HitTarget target;
        private readonly Win2DRenderNode renderNode;

        public HitTarget Target
        {
            get { return target; }
        }

        public Win2DRenderNode RenderNode
        {
            get { return renderNode; }
        }

        public HitResult(Win2DRenderNode renderNode, HitTarget target)
        {
            Guard.NotNull(renderNode, nameof(renderNode));

            this.renderNode = renderNode;

            this.target = target;
        }
    }
}
