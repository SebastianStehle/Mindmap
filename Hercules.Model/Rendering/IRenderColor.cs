// ==========================================================================
// IRenderIcon.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using Windows.UI;

namespace Hercules.Model.Rendering
{
    public interface IRenderColor
    {
        Color Normal { get; }

        Color Darker { get; }

        Color Lighter { get; }
    }
}
