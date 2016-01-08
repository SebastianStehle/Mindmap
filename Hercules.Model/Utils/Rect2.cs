// ==========================================================================
// Rect2.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System;
using System.Collections.Generic;
using System.Numerics;
using Windows.Foundation;

// ReSharper disable ImpureMethodCallOnReadonlyValueField

namespace Hercules.Model.Utils
{
    public struct Rect2 : IEquatable<Rect2>
    {
        public static readonly Rect2 Empty = new Rect2(float.PositiveInfinity, float.PositiveInfinity, float.NegativeInfinity, float.NegativeInfinity);
        public static readonly Rect2 Infinite = new Rect2(float.NegativeInfinity, float.NegativeInfinity, float.PositiveInfinity, float.PositiveInfinity);

        private readonly Vector2 position;
        private readonly Vector2 size;

        public Vector2 Position
        {
            get { return position; }
        }

        public Vector2 Size
        {
            get { return size; }
        }

        public Vector2 Center
        {
            get { return new Vector2(CenterX, CenterY); }
        }

        public float Area
        {
            get { return size.X * size.Y; }
        }

        public float X
        {
            get { return position.X; }
        }

        public float Y
        {
            get { return position.Y; }
        }

        public float Left
        {
            get { return position.X; }
        }

        public float Top
        {
            get { return position.Y; }
        }

        public float Right
        {
            get { return position.X + size.X; }
        }

        public float Bottom
        {
            get { return position.Y + size.Y; }
        }

        public float Width
        {
            get { return size.X; }
        }

        public float Height
        {
            get { return size.Y; }
        }

        public float CenterX
        {
            get { return position.X + (0.5f * size.X); }
        }

        public float CenterY
        {
            get { return position.Y + (0.5f * size.Y); }
        }

        public bool IsEmpty
        {
            get { return size.X < 0 || size.Y < 0; }
        }

        public bool IsInfinite
        {
            get { return float.IsPositiveInfinity(size.X) || float.IsPositiveInfinity(size.X); }
        }

        public Rect2(Vector2 position, Vector2 size)
        {
            this.position = position;

            this.size = size;
        }

        public Rect2(Rect rect)
        {
            position = new Vector2((float)rect.X, (float)rect.Y);

            size = new Vector2((float)rect.Width, (float)rect.Height);
        }

        public Rect2(float x, float y, float w, float h)
        {
            position = new Vector2(x, y);

            size = new Vector2(w, h);
        }

        public Rect2 Inflate(Vector2 v)
        {
            return Inflate(v.X, v.Y);
        }

        public Rect2 Inflate(float width, float height)
        {
            return new Rect2(position.X - width, position.Y - height, size.X + (2 * width), size.Y + (2 * height));
        }

        public static Rect2 Inflate(Rect2 rect, Vector2 size)
        {
            return rect.Inflate(size.X, size.Y);
        }

        public static Rect2 Inflate(Rect2 rect, float width, float height)
        {
            return rect.Inflate(width, height);
        }

        public static Rect2 Deflate(Rect2 rect, Vector2 size)
        {
            return rect.Inflate(-size.X, -size.Y);
        }

        public static Rect2 Deflate(Rect2 rect, float width, float height)
        {
            return rect.Inflate(-width, -height);
        }

        public bool Contains(Size v)
        {
            return Contains((float)v.Width, (float)v.Height);
        }

        public bool Contains(Point v)
        {
            return Contains((float)v.X, (float)v.Y);
        }

        public bool Contains(Vector2 v)
        {
            return Contains(v.X, v.Y);
        }

        public bool Contains(double x, double y)
        {
            return x >= position.X && x - size.X <= position.X && y >= position.Y && y - size.Y <= position.Y;
        }

        public Rect2 Intersect(Rect2 rect)
        {
            if (!IntersectsWith(rect))
            {
                return Empty;
            }

            float minX = Math.Max(X, rect.X);
            float minY = Math.Max(Y, rect.Y);

            float w = Math.Min(position.X + size.X, rect.Position.X + rect.Size.X) - minX;
            float h = Math.Min(position.Y + size.Y, rect.Position.Y + rect.Size.Y) - minY;

            return new Rect2(minX, minY, Math.Max(w, 0.0f), Math.Max(h, 0.0f));
        }

        public bool IntersectsWith(Rect2 rect)
        {
            return !IsEmpty && !rect.IsEmpty && ((rect.Left <= Right && rect.Right >= Left && rect.Top <= Bottom && rect.Bottom >= Top) || IsInfinite || rect.IsInfinite);
        }

        public override bool Equals(object obj)
        {
            return obj is Rect2 && Equals((Rect2)obj);
        }

        public bool Equals(Rect2 other)
        {
            return other.position.Equals(position) && other.size.Equals(size);
        }

        public override int GetHashCode()
        {
            return size.GetHashCode() ^ position.GetHashCode();
        }

        public static bool operator ==(Rect2 lhs, Rect2 rhs)
        {
            return lhs.position.Equals(rhs.position) && lhs.size.Equals(rhs.size);
        }

        public static bool operator !=(Rect2 lhs, Rect2 rhs)
        {
            return !lhs.position.Equals(rhs.position) || !lhs.size.Equals(rhs.size);
        }

        public static implicit operator Rect(Rect2 rect)
        {
            return new Rect(rect.X, rect.Y, rect.Width, rect.Height);
        }

        public IEnumerable<Vector2> ToCorners()
        {
            yield return new Vector2(Left, Top);
            yield return new Vector2(Left, Bottom);
            yield return new Vector2(Right, Top);
            yield return new Vector2(Right, Bottom);
        }

        public override string ToString()
        {
            return $"X: {position.X}, Y: {position.Y}, W: {size.X}, Height: {size.Y}";
        }
    }
}
