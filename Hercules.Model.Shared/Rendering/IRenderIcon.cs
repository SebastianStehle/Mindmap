// ==========================================================================
// IRenderIcon.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System.IO;
using System.Threading.Tasks;

namespace Hercules.Model.Rendering
{
    public interface IRenderIcon
    {
        string Name { get; }

        Task<Stream> ToStreamAsync();
    }
}
