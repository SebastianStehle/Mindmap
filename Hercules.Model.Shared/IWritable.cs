// ==========================================================================
// IWritable.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using GP.Utils;

namespace Hercules.Model
{
    public interface IWritable
    {
        void Save(PropertiesBag properties);
    }
}
