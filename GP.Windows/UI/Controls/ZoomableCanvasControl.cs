// ==========================================================================
// ZoomableCanvasControl.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System;
using System.Numerics;
using Windows.ApplicationModel;
using Windows.Graphics.Display;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.UI.Xaml;

namespace GP.Windows.UI.Controls
{
    /// <summary>
    /// Defines a custom canvas control that supports zooming.
    /// </summary>
    [TemplatePart(Name = SwapChainPanelPart, Type = typeof(CanvasSwapChainPanel))]
    public sealed class ZoomableCanvasControl : Control, ICanvasControl
    {
        private const string SwapChainPanelPart = "PART_SwapChainPanel";
        private CanvasSwapChainPanel swapChainPanel;
        private CanvasSwapChain swapChain;
        private bool mustRegisterEvent = true;
        private float scaleX;
        private float scaleY;

        /// <summary>
        /// Identifies the <see cref="ClearColor"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ClearColorProperty =
            DependencyProperty.Register(nameof(ClearColor), typeof(Color), typeof(ZoomableCanvasControl), new PropertyMetadata(Colors.Transparent, OnClearColorChanged));
        /// <summary>
        /// Gets or sets the color that the control is cleared to before the Draw event is raised.
        /// </summary>
        /// <value>
        /// The color that the control is cleared to before the Draw event is raised.
        /// </value>
        public Color ClearColor
        {
            get { return (Color)GetValue(ClearColorProperty); }
            set { SetValue(ClearColorProperty, value); }
        }

        /// <summary>
        /// Gets the canvas device.
        /// </summary>
        public CanvasDevice Device
        {
            get
            {
                return swapChain?.Device;
            }
        }

        private static void OnClearColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((ZoomableCanvasControl)d).Invalidate();
        }

        /// <summary>
        /// Occurs when the resources must be created.
        /// </summary>
        public event EventHandler CreateResources;

        /// <summary>
        /// This is where the magic happens! Hook this event to issue your immediate mode 2D drawing calls.
        /// </summary>
        public event EventHandler<CanvasDrawEventArgs> Draw;

        /// <summary>
        /// Initializes a new instance of the <see cref="ZoomableCanvasControl"/> class. 
        /// </summary>
        public ZoomableCanvasControl()
        {
            DefaultStyleKey = typeof(ZoomableCanvasControl);
        }

        /// <summary>
        /// Converts the dips to pixels.
        /// </summary>
        /// <param name="dips">The dips to convert.</param>
        /// <param name="dpiRounding">The rounding mode.</param>
        /// <returns>
        /// The resulting pixels.
        /// </returns>
        public int ConvertDipsToPixels(float dips, CanvasDpiRounding dpiRounding)
        {
            return swapChain.ConvertDipsToPixels(dips, dpiRounding);
        }

        /// <summary>
        /// Invoked whenever application code or internal processes (such as a rebuilding layout pass) call ApplyTemplate. 
        /// In simplest terms, this means the method is called just before a UI element displays in your app. 
        /// Override this method to influence the default post-template logic of a class. 
        /// </summary>
        protected override void OnApplyTemplate()
        {
            swapChainPanel = GetTemplateChild(SwapChainPanelPart) as CanvasSwapChainPanel;

            if (swapChainPanel != null)
            {
                swapChainPanel.SizeChanged += (sender, e) =>
                {
                    ResizeOrCreateSwapChain();
                };

                swapChainPanel.CompositionScaleChanged += (sender, e) =>
                {
                    ResizeOrCreateSwapChain();
                };

                CreateResources?.Invoke(this, EventArgs.Empty);
            }
        }

        private void ResizeOrCreateSwapChain()
        {
            if (DesignMode.DesignModeEnabled)
            {
                return;
            }

            if (swapChainPanel != null)
            {
                CanvasDevice device = swapChain != null ? swapChain.Device : CanvasDevice.GetSharedDevice(false);

                scaleX = swapChainPanel.CompositionScaleX;
                scaleY = swapChainPanel.CompositionScaleY;

                float w = scaleX * (float)swapChainPanel.ActualWidth;
                float h = scaleY * (float)swapChainPanel.ActualHeight;

                if (w > 0 && h > 0)
                {
                    float aspectRatio = scaleX / scaleY;

                    if (w > h)
                    {
                        if (w > device.MaximumBitmapSizeInPixels || h > device.MaximumBitmapSizeInPixels)
                        {
                            w = device.MaximumBitmapSizeInPixels;
                            h = device.MaximumBitmapSizeInPixels / aspectRatio;
                        }
                    }
                    else
                    {
                        if (w > device.MaximumBitmapSizeInPixels || h > device.MaximumBitmapSizeInPixels)
                        {
                            h = device.MaximumBitmapSizeInPixels;
                            w = device.MaximumBitmapSizeInPixels * aspectRatio;
                        }
                    }

                    scaleX = w / (float)swapChainPanel.ActualWidth;
                    scaleY = h / (float)swapChainPanel.ActualHeight;

                    if (swapChain == null)
                    {
                        float dpi = DisplayInformation.GetForCurrentView().LogicalDpi;

                        swapChainPanel.SwapChain = swapChain = new CanvasSwapChain(device, w, h, dpi);
                    }
                    else
                    {
                        swapChain.ResizeBuffers(w, h);
                    }

                    swapChain.TransformMatrix =
                        Matrix3x2.CreateScale(
                            1f / scaleX,
                            1f / scaleY);

                    Invalidate();
                }
            }
        }

        private void CompositionTarget_Rendering(object sender, object e)
        {
            CompositionTarget.Rendering -= CompositionTarget_Rendering;

            mustRegisterEvent = true;

            if (swapChainPanel != null && swapChain != null)
            {
                EventHandler<CanvasDrawEventArgs> draw = Draw;

                if (draw != null)
                {
                    using (CanvasDrawingSession session = swapChain.CreateDrawingSession(ClearColor))
                    {
                        session.Transform = Matrix3x2.CreateScale(scaleX, scaleY);

                        draw(this, new CanvasDrawEventArgs(session));
                    }

                    swapChain.Present();
                }
            }
        }

        /// <summary>
        /// Indicates that the contents of the CanvasControl need to be redrawn.
        ///  Calling <see cref="Invalidate"/> results in 
        /// the <see cref="Draw"/> event being raised shortly afterward.
        /// </summary>
        public void Invalidate()
        {
            if (mustRegisterEvent)
            {
                CompositionTarget.Rendering += CompositionTarget_Rendering;

                mustRegisterEvent = false;
            }
        }
    }
}
