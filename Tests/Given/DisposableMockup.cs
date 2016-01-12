// ==========================================================================
// DisposableMockup.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using GP.Utils;

namespace Tests.Given
{
    public class DisposableMockup : DisposableObject
    {
        public int DisposeCount { get; private set; }

        protected override void DisposeObject(bool disposing)
        {
            DisposeCount++;
        }

        public void Foo()
        {
            ThrowIfDisposed();
        }
    }
}
