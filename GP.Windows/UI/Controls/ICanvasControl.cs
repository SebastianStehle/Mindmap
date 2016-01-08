// ==========================================================================
// ICanvasControl.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System;
using Windows.UI.Core;
using Microsoft.Graphics.Canvas;

namespace GP.Windows.UI.Controls
{
    /// <summary>
    /// Interface for different canvas implementaitons.
    /// </summary>
    public interface ICanvasControl
    {
        /// <summary>
        /// Provides access to the core dispatcher associated to this object.
        /// </summary>
        CoreDispatcher Dispatcher { get; }

        /// <summary>
        /// Gets the canvas device.
        /// </summary>
        CanvasDevice Device { get; }

        /// <summary>
        /// Gets or sets a scaling factor applied to this control's Dpi.
        /// </summary>
        float DpiScale { get; set; }

        /// <summary>
        /// Converts the dips to pixels.
        /// </summary>
        /// <param name="dips">The dips to convert.</param>
        /// <param name="dpiRounding">The rounding mode.</param>
        /// <returns>
        /// The resulting pixels.
        /// </returns>
        int ConvertDipsToPixels(float dips, CanvasDpiRounding dpiRounding);

        /// <summary>
        /// This occurs before a drawing operation.
        /// </summary>
        event EventHandler BeforeDraw;

        /// <summary>
        /// This occurs before a drawing operation.
        /// </summary>
        event EventHandler AfterDraw;

        /// <summary>
        /// This is where the magic happens! Hook this event to issue your immediate mode 2D drawing calls.
        /// </summary>
        event EventHandler<BoundedCanvasDrawEventArgs> Draw;

        /// <summary>
        /// Occurs when the resources must be created.
        /// </summary>
        event EventHandler CreateResources;

        /// <summary>
        /// Indicates that the contents of the CanvasControl need to be redrawn.
        ///  Calling <see cref="Invalidate"/> results in 
        /// the <see cref="Draw"/> event being raised shortly afterward.
        /// </summary>
        void Invalidate();
    }
}
