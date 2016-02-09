// ==========================================================================
// ToggleCheckableCommand.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using GP.Utils;

// ReSharper disable UnusedParameter.Local

namespace Hercules.Model
{
    public sealed class ToggleCheckableCommand : IUndoRedoCommand
    {
        private readonly Document document;

        public ToggleCheckableCommand(PropertiesBag properties, Document document)
            : this(document)
        {
        }

        public ToggleCheckableCommand(Document document)
        {
            Guard.NotNull(document, nameof(document));

            this.document = document;
        }

        private void Execute(bool isRedo)
        {
            document.ChangeIsCheckableDefault(!document.IsCheckableDefault);
        }

        private void Revert()
        {
            Execute(false);
        }

        public void Undo()
        {
            Revert();
        }

        public void Redo()
        {
            Execute(true);
        }

        public void Execute()
        {
            Execute(false);
        }

        public void Save(PropertiesBag properties)
        {
        }
    }
}
