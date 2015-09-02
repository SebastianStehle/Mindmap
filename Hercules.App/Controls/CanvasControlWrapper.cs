// ==========================================================================
// CanvasControlWrapper.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

// ReSharper disable UnusedParameter.Local

using System;
using Windows.UI.Core;
using GP.Windows;
using GP.Windows.UI.Controls;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.UI.Xaml;

namespace Hercules.App.Controls
{
    class CanvasControlWrapper : ICanvasControl
    {
        private readonly CanvasControl inner;

        public CanvasDevice Device
        {
            get { return inner.Device; }
        }

        public CoreDispatcher Dispatcher
        {
            get { return inner.Dispatcher; }
        }

        public event EventHandler<CanvasDrawEventArgs> Draw;

        protected void OnDraw(CanvasDrawEventArgs e)
        {
            EventHandler<CanvasDrawEventArgs> eventHandler = Draw;

            if (eventHandler != null)
            {
                eventHandler(this, e);
            }
        }

        public CanvasControlWrapper(CanvasControl canvasControl)
        {
            Guard.NotNull(canvasControl, nameof(canvasControl));

            inner = canvasControl;

            inner.Draw += Inner_Draw;
        }

        private void Inner_Draw(CanvasControl sender, CanvasDrawEventArgs args)
        {
            OnDraw(args);
        }

        public int ConvertDipsToPixels(float dips, CanvasDpiRounding dpiRounding)
        {
            return inner.ConvertDipsToPixels(dips, dpiRounding);
        }

        public void Invalidate()
        {
            if (inner.ReadyToDraw)
            {
                inner.Invalidate();
            }
            else
            {
#pragma warning disable 4014
                Dispatcher.RunIdleAsync(x => inner.Invalidate());
#pragma warning restore 4014
            }
        }
    }
}
