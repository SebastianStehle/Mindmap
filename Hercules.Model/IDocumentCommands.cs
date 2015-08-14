// ==========================================================================
// IDocumentCommands.cs
// Hercules Mindmap App
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
