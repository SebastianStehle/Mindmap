// ==========================================================================
// ConvexHull.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using GP.Windows;

namespace Hercules.Model.Utils
{
    public sealed class ConvexHull
    {
        private readonly List<Vector2> points;
        
        public IReadOnlyList<Vector2> Points
        {
            get
            {
                return points;
            }
        }

        public ConvexHull(List<Vector2> points)
        {
            Guard.NotNull(points, nameof(points));

            this.points = points;
        }

        public ConvexHull Extent(float padding)
        {
            int size = points.Count;

            List<Vector2> hull = new List<Vector2>(size);

            int e = size - 1;

            for (int i = 0; i < size; i++)
            {
                Vector2 c = points[i], n, p;

                p = i > 0 ? points[i - 1] : points[e];
                n = i < e ? points[i + 1] : points[0];

                Vector2 v = Vector2.Normalize((p - c) + (n - c));

                hull.Add(p - v * padding);
            }

            return new ConvexHull(hull);
        }

        public static ConvexHull Compute(IEnumerable<Rect2> bounds)
        {
            Guard.NotNull(bounds, nameof(bounds));

            return Compute(bounds.SelectMany(x => x.ToCorners()).ToList());
        }

        public static ConvexHull Compute(List<Vector2> points)
        {
            Guard.NotNull(points, nameof(points));

            List<Vector2> sorted = new List<Vector2>(points);

            sorted.Sort((l, r) => l.X == r.X ? l.Y.CompareTo(r.Y) : (l.X > r.X ? 1 : -1));

            return ComputeSorted(sorted);
        }

        public static ConvexHull ComputeSorted(List<Vector2> points)
        {
            Guard.NotNull(points, nameof(points));

            List<Vector2> hull = new List<Vector2>();

            int lower = 0, upper = 0;

            for (int i = points.Count - 1; i >= 0; i--)
            {
                Vector2 p = points[i], p1;

                while (lower >= 2)
                {
                    p1 = hull[hull.Count - 1];

                    if (Cross(p1 - hull[hull.Count - 2], p - p1) < 0)
                    {
                        break;
                    }

                    hull.RemoveAt(hull.Count - 1);
                    lower--;
                }

                hull.Add(p);
                lower++;

                while (upper >= 2)
                {
                    p1 = hull[0];

                    if (Cross(p1 - hull[1], p - p1) > 0)
                    {
                        break;
                    }

                    hull.RemoveAt(0);
                    upper--;
                }

                if (upper != 0)
                {
                    hull.Insert(0, p);
                }

                upper++;
            }

            hull.RemoveAt(hull.Count - 1);

            return new ConvexHull(hull);
        }

        private static double Cross(Vector2 a, Vector2 b)
        {
            return a.X * b.Y - a.Y * b.X;
        }
    }
}
