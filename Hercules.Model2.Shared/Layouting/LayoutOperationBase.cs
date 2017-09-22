// ==========================================================================
// LayoutOperationBase.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System.Numerics;
using GP.Utils;
using Hercules.Model2.Rendering;

namespace Hercules.Model2.Layouting
{
    public abstract class LayoutOperation<TLayout> where TLayout : ILayout
    {
        private readonly TLayout layout;
        private readonly IRenderScene scene;
        private readonly Document document;
        private readonly Vector2 mindmapCenter;

        protected TLayout Layout
        {
            get { return layout; }
        }

        protected IRenderScene Scene
        {
            get { return scene; }
        }

        protected Document Document
        {
            get { return document; }
        }

        public Vector2 MindmapCenter
        {
            get { return mindmapCenter; }
        }

        protected LayoutOperation(TLayout layout, IRenderScene scene, Document document)
        {
            Guard.NotNull(scene, nameof(scene));
            Guard.NotNull(layout, nameof(layout));
            Guard.NotNull(document, nameof(document));

            this.scene = scene;
            this.layout = layout;
            this.document = document;

            mindmapCenter = CalculateCenter(document);
        }

        private static Vector2 CalculateCenter(Document document)
        {
            var x = 0.5f * document.Size.X;
            var y = 0.5f * document.Size.Y;

            return new Vector2(x, y);
        }
    }
}
