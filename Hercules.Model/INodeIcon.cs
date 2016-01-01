// ==========================================================================
// INodeIcon.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System;
using System.Threading.Tasks;
using Windows.Storage.Streams;

namespace Hercules.Model
{
    public interface INodeIcon : IEquatable<INodeIcon>, IWritable
    {
        string Name { get; }

        int PixelWidth { get; }

        int PixelHeight { get; }

        Task<IRandomAccessStream> OpenAsStreamAsync();
    }
}
