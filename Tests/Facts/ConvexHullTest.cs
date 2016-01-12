// ==========================================================================
// ConvexHullTest.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using GP.Utils.Mathematics;
using Xunit;

namespace Tests.Facts
{
    public class ConvexHullTest
    {
        [Fact]
        public void ComputeHull()
        {
            Rect2 rect1 = new Rect2(00, 00, 50, 50);
            Rect2 rect2 = new Rect2(50, 50, 20, 20);

            IEnumerable<Rect2> rects = new[] { rect1, rect2 };

            ConvexHull hull = ConvexHull.Compute(rects);

            Assert.Equal(6, hull.Points.Count);

            Assert.Equal(new[]
            {
                new Vector2(00, 00),
                new Vector2(00, 50),
                new Vector2(50, 70),
                new Vector2(70, 70),
                new Vector2(70, 50),
                new Vector2(50, 00)
            }, hull.Points.ToArray());
        }
    }
}
