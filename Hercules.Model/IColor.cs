// ==========================================================================
// IColor.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System;

namespace Hercules.Model
{
    public interface IColor : IEquatable<IColor>
    {
        void Save(PropertiesBag properties);
    }
}
