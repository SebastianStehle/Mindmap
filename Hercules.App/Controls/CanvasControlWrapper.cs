// ==========================================================================
// CanvasControlWrapper.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System;
using Windows.Foundation;
using Windows.UI.Core;
using GP.Utils;
using GP.Utils.Mathematics;
using GP.Utils.UI.Controls;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.UI.Xaml;

namespace Hercules.App.Controls
{
    public sealed class CanvasControlWrapper : ICanvasControl
    {
        private readonly CanvasVirtualControl inner;

        public CanvasDevice Device
        {
            get { return inner.Device; }
        }

        public CanvasVirtualControl Inner
        {
            get { return inner; }
        }

        public CoreDispatcher Dispatcher
        {
            get { return inner.Dispatcher; }
        }

        public float DpiScale
        {
            get { return inner.DpiScale; }
            set { inner.DpiScale = value; }
        }

        public event EventHandler<BoundedCanvasDrawEventArgs> Draw;

        public event EventHandler BeforeDraw;

        public event EventHandler AfterDraw;

        public event EventHandler CreateResources;

        private void OnDraw(BoundedCanvasDrawEventArgs e)
        {
            Draw?.Invoke(this, e);
        }

        private void OnBeforeDraw()
        {
            BeforeDraw?.Invoke(this, EventArgs.Empty);
        }

        private void OnAfterDraw()
        {
            AfterDraw?.Invoke(this, EventArgs.Empty);
        }

        private void OnCreateResources()
        {
            CreateResources?.Invoke(this, EventArgs.Empty);
        }

        public CanvasControlWrapper(CanvasVirtualControl canvasControl)
        {
            Guard.NotNull(canvasControl, nameof(canvasControl));

            inner = canvasControl;

            inner.CreateResources += (sender, args) =>
            {
                OnCreateResources();
            };

            inner.RegionsInvalidated += (sender, args) =>
            {
                if (args.InvalidatedRegions.Length > 0)
                {
                    OnBeforeDraw();

                    foreach (Rect region in args.InvalidatedRegions)
                    {
                        using (CanvasDrawingSession session = canvasControl.CreateDrawingSession(region))
                        {
                            OnDraw(new BoundedCanvasDrawEventArgs(session, region.ToRect2()));
                        }
                    }

                    OnAfterDraw();
                }
            };
        }

        public int ConvertDipsToPixels(float dips, CanvasDpiRounding dpiRounding)
        {
            return inner.ConvertDipsToPixels(dips, dpiRounding);
        }

        public void Invalidate()
        {
            Dispatcher.RunIdleAsync(x => inner.Invalidate()).Forget();
        }
    }
}
