// ==========================================================================
// DisposableObjectTest.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System;
using Tests.Given;
using Xunit;

// ReSharper disable AccessToDisposedClosure

namespace Tests.Facts
{
    public class DisposableObjectTest
    {
        [Fact]
        public void DisposeObject()
        {
            DisposableMockup target = new DisposableMockup();

            target.Foo();

            Assert.False(target.IsDisposed);

            target.Dispose();

            Assert.Equal(1, target.DisposeCount);
            Assert.True(target.IsDisposed);

            Assert.Throws<ObjectDisposedException>(() => target.Foo());

            target.Dispose();

            Assert.Equal(1, target.DisposeCount);
        }
    }
}
