// ==========================================================================
// IDocumentCommands.cs
// Mindmap Application
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

namespace MindmapApp.Model
{
    public interface IDocumentCommands
    {
        void Apply(CommandBase command);
    }
}
