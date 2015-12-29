﻿// ==========================================================================
// Win2DRenderable.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System.Numerics;
using GP.Windows;
using Hercules.Model;
using Hercules.Model.Rendering;
using Hercules.Model.Utils;

namespace Hercules.Win2D.Rendering
{
    public abstract class Win2DRenderable : IRenderable
    {
        private Vector2 renderPosition;
        private Vector2 renderSize;
        private Rect2 renderBounds;
        private readonly NodeBase node;
        private readonly Win2DRenderer renderer;

        public NodeBase Node
        {
            get { return node; }
        }

        public Win2DRenderer Renderer
        {
            get { return renderer; }
        }

        public Win2DResourceManager Resources
        {
            get { return renderer.Resources; }
        }

        public Win2DScene Scene
        {
            get { return renderer.Scene; }
        }

        public Vector2 RenderPosition
        {
            get { return renderPosition; }
        }

        public Vector2 RenderSize
        {
            get { return renderSize; }
        }

        public Rect2 RenderBounds
        {
            get { return renderBounds; }
        }

        protected Win2DRenderable(NodeBase node, Win2DRenderer renderer)
        {
            Guard.NotNull(node, nameof(node));
            Guard.NotNull(renderer, nameof(renderer));

            this.node = node;

            this.renderer = renderer;
        }

        public void UpdateSize(Vector2 size)
        {
            renderSize = size;
        }

        public void UpdatePosition(Vector2 position)
        {
            renderPosition = position;
        }

        public void UpdateBounds()
        {
            renderBounds = new Rect2(renderPosition, renderSize);
        }
    }
}