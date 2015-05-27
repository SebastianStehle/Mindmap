// ==========================================================================
// IDocumentCommands.cs
// Mindmap Application
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

namespace Mindmap.Model
{
    public interface IDocumentCommands
    {
        void Apply(CommandBase command);
    }
}
