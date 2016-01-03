// ==========================================================================
// Win2DRenderer.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System;
using System.Numerics;
using System.Threading.Tasks;
using Windows.Graphics.Printing;
using Windows.Storage.Streams;
using Windows.UI;
using GP.Windows;
using GP.Windows.UI.Controls;
using Hercules.Model;
using Hercules.Model.Layouting;
using Hercules.Model.Rendering;
using Hercules.Model.Utils;
using Hercules.Win2D.Rendering.Utils;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.UI.Xaml;

// ReSharper disable DoNotCallOverridableMethodsInConstructor

namespace Hercules.Win2D.Rendering
{
    public abstract class Win2DRenderer : DisposableObject, IRenderer
    {
        private readonly Win2DResourceManager resources;
        private readonly Document document;
        private readonly Win2DScene scene;
        private readonly Win2DSceneTransformator transformator = new Win2DSceneTransformator();
        private readonly ICanvasControl canvas;
        private Rect2 visibleRect;
        private ILayout layout;

        public float ZoomFactor
        {
            get { return transformator.ZoomFactor; }
        }

        public Win2DScene Scene
        {
            get { return scene; }
        }

        public Document Document
        {
            get { return document; }
        }

        public Win2DResourceManager Resources
        {
            get { return resources; }
        }

        public ICanvasControl Canvas
        {
            get { return canvas; }
        }

        public ILayout Layout
        {
            get
            {
                return layout;
            }
            set
            {
                Guard.NotNull(value, nameof(value));

                if (layout != value)
                {
                    layout = value;

                    Invalidate();
                }
            }
        }

        protected Win2DRenderer(Document document, ICanvasControl canvas)
        {
            Guard.NotNull(document, nameof(document));
            Guard.NotNull(canvas, nameof(canvas));

            this.canvas = canvas;
            this.document = document;

            scene = new Win2DScene(document, CreatePreviewNode(), CreateRenderNode);

            InitializeCanvas();
            InitializeDocument();

            resources = new Win2DResourceManager(canvas);
        }

        protected override void DisposeObject(bool disposing)
        {
            scene.Dispose();

            ReleaseDocument();
            ReleaseCanvas();
        }

        public void Transform(Vector2 translate, float zoom, Rect2 visible)
        {
            visibleRect = visible;

            transformator.Transform(translate, zoom);
        }

        private void InitializeDocument()
        {
            document.StateChanged += Document_StateChanged;
            document.NodeSelected += Document_NodeSelected;
        }

        private void ReleaseDocument()
        {
            document.StateChanged -= Document_StateChanged;
            document.NodeSelected -= Document_NodeSelected;
        }

        private void InitializeCanvas()
        {
            canvas.CreateResources += Canvas_CreateResources;

            canvas.Draw += Canvas_Draw;
        }

        private void ReleaseCanvas()
        {
            canvas.CreateResources -= Canvas_CreateResources;

            canvas.Draw -= Canvas_Draw;
        }

        private void Canvas_CreateResources(object sender, EventArgs e)
        {
            resources.ClearResources();

            scene.ClearResources();

            Invalidate();
        }

        public void HidePreviewElement()
        {
            scene.HidePreviewElement();
        }

        public void ShowPreviewElement(Vector2 position, NodeBase parent, NodeSide anchor)
        {
            scene.ShowPreviewElement(position, parent, anchor);
        }

        public void Invalidate()
        {
            scene.InvalidateLayout();

            canvas.Invalidate();
        }

        public void InvalidateWithoutLayout()
        {
            canvas.Invalidate();
        }

        private void Canvas_Draw(object sender, CanvasDrawEventArgs args)
        {
            RenderForUI(args.DrawingSession);
        }

        public IPrintDocumentSource Print(float padding = 20)
        {
            return Printer.Print(scene, padding);
        }

        public Task RenderScreenshotAsync(IRandomAccessStream stream, Color background, float? dpi = null, float padding = 20)
        {
            return ScreenshotMaker.RenderScreenshotAsync(scene, canvas.Device, stream, background, dpi, padding);
        }

        public void RemoveAdorner(IAdornerRenderNode adorner)
        {
            scene.RemoveAdorner(adorner);
        }

        public IAdornerRenderNode CreateAdorner(IRenderNode renderNode)
        {
            return scene.CreateAdorner(renderNode);
        }

        private void RenderForUI(CanvasDrawingSession session)
        {
            if (layout != null)
            {
                transformator.Transform(session);

                bool needsRedraw;

                scene.UpdateLayout(session, layout);
                scene.UpdateArrangement(session, true, out needsRedraw);

                scene.Render(session, true, visibleRect);

                if (needsRedraw)
                {
                    canvas.Invalidate();
                }
            }
        }

        public bool HandleClick(Vector2 hitPosition, out Win2DRenderNode handledNode)
        {
            bool isHit = scene.HandleClick(hitPosition, out handledNode);

            if (isHit)
            {
                Invalidate();
            }

            return isHit;
        }

        public Vector2 GetMindmapSize(Vector2 size)
        {
            return transformator.GetMindmapSize(size);
        }

        public Vector2 GetMindmapPosition(Vector2 position)
        {
            return transformator.GetMindmapPosition(position);
        }

        public Vector2 GetOverlaySize(Vector2 size)
        {
            return transformator.GetOverlaySize(size);
        }

        public Vector2 GetOverlayPosition(Vector2 position)
        {
            return transformator.GetOverlayPosition(position);
        }

        private void Document_NodeSelected(object sender, NodeEventArgs e)
        {
            InvalidateWithoutLayout();
        }

        private void Document_StateChanged(object sender, EventArgs e)
        {
            Invalidate();
        }

        public IRenderColor FindColor(NodeBase node)
        {
            return resources.FindColor(node);
        }

        public IRenderIcon FindIcon(NodeBase node)
        {
            return resources.FindIcon(node);
        }

        protected abstract Win2DRenderNode CreatePreviewNode();

        protected abstract Win2DRenderNode CreateRenderNode(NodeBase node);
    }
}
