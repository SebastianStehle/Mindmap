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
using Windows.UI.Xaml;
using GP.Utils;
using GP.Utils.Mathematics;
using GP.Utils.UI.Controls;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.UI.Xaml;

namespace Hercules.App.Controls
{
    public sealed class CanvasControlWrapper : ICanvasControl
    {
        private readonly CanvasVirtualControl canvasControl;

        public CanvasDevice Device
        {
            get { return canvasControl.Device; }
        }

        public CanvasVirtualControl Inner
        {
            get { return canvasControl; }
        }

        public CoreDispatcher Dispatcher
        {
            get { return canvasControl.Dispatcher; }
        }

        public float DpiScale
        {
            get
            {
                return canvasControl.DpiScale;
            }
            set
            {
                canvasControl.DpiScale = value;
            }
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

            this.canvasControl = canvasControl;

            canvasControl.CreateResources += (sender, args) =>
            {
                OnCreateResources();
            };

            canvasControl.RegionsInvalidated += (sender, args) =>
            {
                if (args.InvalidatedRegions.Length <= 0)
                {
                    return;
                }

                OnBeforeDraw();

                foreach (Rect region in args.InvalidatedRegions)
                {
                    using (CanvasDrawingSession session = canvasControl.CreateDrawingSession(region))
                    {
                        OnDraw(new BoundedCanvasDrawEventArgs(session, region.ToRect2()));
                    }
                }

                OnAfterDraw();
            };

            Application.Current.Resuming += App_Resuming;
        }

        private void App_Resuming(object sender, object e)
        {
            Invalidate();
        }

        public int ConvertDipsToPixels(float dips, CanvasDpiRounding dpiRounding)
        {
            return canvasControl.ConvertDipsToPixels(dips, dpiRounding);
        }

        public void Invalidate()
        {
            Dispatcher.RunIdleAsync(x => canvasControl.Invalidate()).Forget();
        }
    }
}
