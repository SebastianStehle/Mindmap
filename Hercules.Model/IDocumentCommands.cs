// ==========================================================================
// IDocumentCommands.cs
// Hercules Application
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

namespace Hercules.Model
{
    public interface IDocumentCommands
    {
        void Apply(CommandBase command);
    }
}
