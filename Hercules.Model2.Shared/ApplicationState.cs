// ==========================================================================
// ApplicationState.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System;

namespace Hercules.Model2
{
    public sealed class ApplicationState : Cloneable<ApplicationState>
    {
        private UndoableState<Document> document;

        public UndoableState<Document> Document
        {
            get { return document; }
        }

        public ApplicationState()
        {
            document = new UndoableState<Document>(20).ReplacePresent(new Document(Guid.NewGuid()));
        }

        public ApplicationState WithDocument(UndoableState<Document> newDocument)
        {
            if (newDocument == document || newDocument == null)
            {
                return this;
            }

            return Clone(s => s.document = newDocument);
        }
    }
}
