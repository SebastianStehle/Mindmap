// ==========================================================================
// DocumentTest.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System;
using Hercules.Model;
using Hercules.Model.Utils;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;

namespace UnitTests.Tests
{
    [TestClass]
    public class DocumentTest
    {
        private readonly Document document = new Document(Guid.NewGuid());

        public DocumentTest()
        {
            ResourceManager.StringProvider = x => x;
        }

        [TestMethod]
        public void NodeAddedToRoot_RightThenLeft_Undo_Removed()
        {
            document.Root.AddChildTransactional();

            Assert.AreEqual(1, document.Root.RightChildren.Count);
            Assert.AreEqual(0, document.Root.LeftChildren.Count);

            document.Root.AddChildTransactional();

            Assert.AreEqual(1, document.Root.RightChildren.Count);
            Assert.AreEqual(1, document.Root.LeftChildren.Count);

            document.Root.AddChildTransactional();

            Assert.AreEqual(2, document.Root.RightChildren.Count);
            Assert.AreEqual(1, document.Root.LeftChildren.Count);

            document.UndoRedoManager.Revert();

            Assert.AreEqual(1, document.Root.RightChildren.Count);
            Assert.AreEqual(1, document.Root.LeftChildren.Count);

            document.UndoRedoManager.Revert();

            Assert.AreEqual(1, document.Root.RightChildren.Count);
            Assert.AreEqual(0, document.Root.LeftChildren.Count);

            document.UndoRedoManager.Revert();

            Assert.AreEqual(0, document.Root.RightChildren.Count);
            Assert.AreEqual(0, document.Root.LeftChildren.Count);
        }
    }
}
