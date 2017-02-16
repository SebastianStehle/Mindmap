// ==========================================================================
// DocumentTest.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System;
using GP.Utils;
using Hercules.Model;
using Xunit;

namespace Tests.Facts
{
    public class DocumentTest
    {
        private readonly Document document = new Document(Guid.NewGuid());

        public DocumentTest()
        {
            LocalizationManager.Provider = new NoopLocalizationProvider();
        }

        [Fact]
        public void NodeAdded_HasDocument()
        {
            var child1 = new Node(Guid.NewGuid());
            var child11 = new Node(Guid.NewGuid());
            var child12 = new Node(Guid.NewGuid());

            child1.Insert(child11, 0, NodeSide.Right);
            child1.Insert(child12, 1, NodeSide.Right);

            document.Root.Insert(child1, null, NodeSide.Auto);

            Assert.Equal(document.Root, child1.Parent);

            Assert.Equal(document, child1.Document);
            Assert.Equal(document, child11.Document);
            Assert.Equal(document, child12.Document);
        }

        [Fact]
        public void NodeAddedToRoot_RightThenLeft_Undo_Removed()
        {
            document.Root.AddChildTransactional();

            Assert.Equal(1, document.Root.RightChildren.Count);
            Assert.Equal(0, document.Root.LeftChildren.Count);

            document.Root.AddChildTransactional();

            Assert.Equal(1, document.Root.RightChildren.Count);
            Assert.Equal(1, document.Root.LeftChildren.Count);

            document.Root.AddChildTransactional();

            Assert.Equal(2, document.Root.RightChildren.Count);
            Assert.Equal(1, document.Root.LeftChildren.Count);

            document.UndoRedoManager.Undo();

            Assert.Equal(1, document.Root.RightChildren.Count);
            Assert.Equal(1, document.Root.LeftChildren.Count);

            document.UndoRedoManager.Undo();

            Assert.Equal(1, document.Root.RightChildren.Count);
            Assert.Equal(0, document.Root.LeftChildren.Count);

            document.UndoRedoManager.Undo();

            Assert.Equal(0, document.Root.RightChildren.Count);
            Assert.Equal(0, document.Root.LeftChildren.Count);
        }
    }
}
