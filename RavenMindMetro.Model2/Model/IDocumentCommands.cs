// ==========================================================================
// IDocumentCommands.cs
// RavenMind Application
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

namespace RavenMind.Model
{
    public interface IDocumentCommands
    {
        void Apply(CommandBase command);
    }
}
