// ==========================================================================
// IRenderColor.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System.Numerics;

namespace Hercules.Model2.Rendering
{
    public interface IRenderColor
    {
        Vector3 Normal { get; }

        Vector3 Darker { get; }

        Vector3 Lighter { get; }
    }
}
