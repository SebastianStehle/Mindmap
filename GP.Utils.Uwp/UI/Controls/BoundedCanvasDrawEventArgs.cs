// ==========================================================================
// BoundedCanvasDrawEventArgs.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System;
using GP.Utils.Mathematics;
using Microsoft.Graphics.Canvas;

namespace GP.Utils.UI.Controls
{
    /// <summary>
    /// Provides data for the draw event.
    /// </summary>
    public sealed class BoundedCanvasDrawEventArgs : EventArgs
    {
        private readonly CanvasDrawingSession drawingSession;
        private readonly Rect2 renderBounds;

        /// <summary>
        /// Gets the render bounds.
        /// </summary>
        public Rect2 RenderBounds
        {
            get { return renderBounds; }
        }

        /// <summary>
        /// Gets the drawing session for use by the current event handler. This provides
        /// methods to draw lines, rectangles, text etc.
        /// </summary>
        public CanvasDrawingSession DrawingSession
        {
            get { return drawingSession; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BoundedCanvasDrawEventArgs"/> class with the drawing session and the render bounds.
        /// </summary>
        /// <param name="drawingSession">The drawing session for use by the current event handler.</param>
        /// <exception cref="ArgumentNullException"><paramref name="drawingSession"/> is null.</exception>
        public BoundedCanvasDrawEventArgs(CanvasDrawingSession drawingSession)
            : this(drawingSession, Rect2.Empty)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BoundedCanvasDrawEventArgs"/> class with the drawing session and the render bounds.
        /// </summary>
        /// <param name="drawingSession">The drawing session for use by the current event handler.</param>
        /// <param name="renderBounds">The render bounds.</param>
        /// <exception cref="ArgumentNullException"><paramref name="drawingSession"/> is null.</exception>
        public BoundedCanvasDrawEventArgs(CanvasDrawingSession drawingSession, Rect2 renderBounds)
        {
            Guard.NotNull(drawingSession, nameof(drawingSession));

            this.drawingSession = drawingSession;
            this.renderBounds = renderBounds;
        }
    }
}
