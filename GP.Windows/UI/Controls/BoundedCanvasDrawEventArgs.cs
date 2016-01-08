// ==========================================================================
// BoundedCanvasDrawEventArgs.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System;
using Windows.Foundation;
using Microsoft.Graphics.Canvas;

namespace GP.Windows.UI.Controls
{
    /// <summary>
    /// Provides data for the draw event.
    /// </summary>
    public sealed class BoundedCanvasDrawEventArgs : EventArgs
    {
        private readonly CanvasDrawingSession drawingSession;
        private readonly Rect renderBounds;

        /// <summary>
        /// Gets the render bounds.
        /// </summary>
        public Rect RenderBounds
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
            : this(drawingSession, Rect.Empty)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BoundedCanvasDrawEventArgs"/> class with the drawing session and the render bounds.
        /// </summary>
        /// <param name="drawingSession">The drawing session for use by the current event handler.</param>
        /// <param name="renderBounds">The render bounds.</param>
        /// <exception cref="ArgumentNullException"><paramref name="drawingSession"/> is null.</exception>
        public BoundedCanvasDrawEventArgs(CanvasDrawingSession drawingSession, Rect renderBounds)
        {
            Guard.NotNull(drawingSession, nameof(drawingSession));

            this.drawingSession = drawingSession;
            this.renderBounds = renderBounds;
        }
    }
}
