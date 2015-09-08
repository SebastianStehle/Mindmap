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

// ReSharper disable NonReadonlyMemberInGetHashCode

namespace Hercules.Model.Utils
{
    public struct Rect2 : IEquatable<Rect2>
    {
        public static readonly Rect2 Empty = new Rect2(float.PositiveInfinity, float.PositiveInfinity, float.NegativeInfinity, float.NegativeInfinity);

        private Vector2 position;
        private Vector2 size;

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
            get { return position.X + 0.5f * size.X; }
        }

        public float CenterY
        {
            get { return position.Y + 0.5f * size.Y; }
        }

        public bool IsEmpty
        {
            get { return size.X < 0; }
        }

        public Rect2(Vector2 position, Vector2 size)
        {
            this.position = position;

            this.size = size;
        }

        public Rect2(float x, float y, float w, float h)
        {
            position = new Vector2(x, y);

            size = new Vector2(w, h);
        }

        public void Inflate(Vector2 v)
        {
            Inflate(v.X, v.Y);
        }

        public void Inflate(float width, float height)
        {
            position.X -= width;
            position.Y -= height;
            size.X += width;
            size.X += width;
            size.Y += height;
            size.Y += height;
        }

        public static Rect2 Inflate(Rect2 rect, Vector2 size)
        {
            rect.Inflate(size.X, size.Y);

            return rect;
        }

        public static Rect2 Inflate(Rect2 rect, float width, float height)
        {
            rect.Inflate(width, height);

            return rect;
        }

        public static Rect2 Deflate(Rect2 rect, Vector2 size)
        {
            rect.Inflate(-size.X, -size.Y);

            return rect;
        }

        public static Rect2 Deflate(Rect2 rect, float width, float height)
        {
            rect.Inflate(-width, -height);

            return rect;
        }

        public void Contains(Size v)
        {
            Inflate((float)v.Width, (float)v.Height);
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

        public IEnumerable<Vector2> ToCorners()
        {
            yield return new Vector2(Left, Top);
            yield return new Vector2(Left, Bottom);
            yield return new Vector2(Right, Top);
            yield return new Vector2(Right, Bottom);
        }

        public void Intersect(Rect2 rect)
        {
            if (!IntersectsWith(rect))
            {
                this = Empty;

                return;
            }

            float minX = Math.Max(X, rect.X);
            float minY = Math.Max(Y, rect.Y);

            float w = Math.Min(position.X + size.X, rect.Position.X + rect.Size.X) - minX;
            float h = Math.Min(position.Y + size.Y, rect.Position.Y + rect.Size.Y) - minY;

            size = new Vector2(Math.Max(w, 0.0f), Math.Max(h, 0.0f));

            position = new Vector2(minX, minY);
        }

        public bool IntersectsWith(Rect2 rect)
        {
            return !IsEmpty && !rect.IsEmpty && (rect.Left <= Right && rect.Right >= Left && rect.Top <= Bottom) && rect.Bottom >= Top;
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

        public static bool operator == (Rect2 lhs, Rect2 rhs)
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

        public override string ToString()
        {
            return $"X: {position.X}, Y: {position.Y}, W: {size.X}, Height: {size.Y}";
        }
    }
}
