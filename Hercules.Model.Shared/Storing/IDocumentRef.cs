// ==========================================================================
// DocumentRef.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System;

namespace Hercules.Model.Storing
{
    public interface IDocumentRef
    {
        string DocumentName { get; }

        DateTimeOffset ModifiedUtc { get; }
    }
}
