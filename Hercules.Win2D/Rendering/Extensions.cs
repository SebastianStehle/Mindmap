// ==========================================================================
// Extensions.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System;
using System.Numerics;
using Microsoft.Graphics.Canvas;

namespace Hercules.Win2D.Rendering
{
    public static class Extensions
    {
        private sealed class TransformReset : IDisposable
        {
            private readonly CanvasDrawingSession session;
            private readonly Matrix3x2 previousTransform;

            public TransformReset(CanvasDrawingSession session)
            {
                this.session = session;

                previousTransform = session.Transform;
            }

            public void Dispose()
            {
                session.Transform = previousTransform;
            }
        }

        public static IDisposable Transform(this CanvasDrawingSession session, Matrix3x2 transform)
        {
            IDisposable reset = new TransformReset(session);

            session.Transform = transform;

            return reset;
        }

        public static IDisposable StackTransform(this CanvasDrawingSession session, Matrix3x2 transform)
        {
            IDisposable reset = new TransformReset(session);

            session.Transform *= transform;

            return reset;
        }
    }
}
