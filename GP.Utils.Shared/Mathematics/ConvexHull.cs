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

namespace GP.Utils.Mathematics
{
    /// <summary>
    /// Defines a convex hull that can be constructed from multiple vectors.
    /// </summary>
    public sealed class ConvexHull
    {
        private readonly List<Vector2> points;

        /// <summary>
        /// The vectors of the convex hull.
        /// </summary>
        public IReadOnlyList<Vector2> Points
        {
            get
            {
                return points;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConvexHull"/> instance with the points of the convex hull.
        /// </summary>
        /// <param name="points">The points of the convex hull. Cannot be null.</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="points"/> is null.</exception>
        public ConvexHull(List<Vector2> points)
        {
            Guard.NotNull(points, nameof(points));

            this.points = points;
        }

        /// <summary>
        /// Computes a new hull from the specified bounds.
        /// </summary>
        /// <param name="bounds">The bounds to create the hull from.</param>
        /// <returns>
        /// The created convex hull.
        /// </returns>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="bounds"/> is null.</exception>
        public static ConvexHull Compute(IEnumerable<Rect2> bounds)
        {
            Guard.NotNull(bounds, nameof(bounds));

            return Compute(bounds.SelectMany(x => x.ToCorners()).ToList());
        }

        /// <summary>
        /// Computes a new hull from the specified points.
        /// </summary>
        /// <param name="points">The points to create the hull from.</param>
        /// <returns>
        /// The created convex hull.
        /// </returns>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="points"/> is null.</exception>
        public static ConvexHull Compute(List<Vector2> points)
        {
            Guard.NotNull(points, nameof(points));

            List<Vector2> sorted = new List<Vector2>(points);

            sorted.Sort((l, r) => System.Math.Abs(l.X - r.X) < float.Epsilon ? l.Y.CompareTo(r.Y) : (l.X > r.X ? 1 : -1));

            return ComputeSorted(sorted);
        }

        /// <summary>
        /// Computes a new hull from the specified sorted points.
        /// </summary>
        /// <param name="points">The sorted points to create the hull from.</param>
        /// <returns>
        /// The created convex hull.
        /// </returns>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="points"/> is null.</exception>
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
            return (a.X * b.Y) - (a.Y * b.X);
        }
    }
}
