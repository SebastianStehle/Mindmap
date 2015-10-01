// ==========================================================================
// LayoutOperationBase.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using GP.Windows;

namespace Hercules.Model.Layouting
{
    public abstract class LayoutOperation<TLayout> where TLayout : ILayout
    {
        private readonly TLayout layout;
        private readonly IRenderScene scene;
        private readonly Document document;

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

        protected LayoutOperation(TLayout layout, IRenderScene scene, Document document)
        {
            Guard.NotNull(scene, nameof(scene));
            Guard.NotNull(layout, nameof(layout));
            Guard.NotNull(document, nameof(document));

            this.scene = scene;
            this.layout = layout;
            this.document = document;
        }
    }
}
