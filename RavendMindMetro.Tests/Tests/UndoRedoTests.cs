// ==========================================================================
// UndoRedoTests.cs
// SD Components
// ==========================================================================
// Copyright (c) Silverlight Shapes Development Group
// All rights reserved.
// ==========================================================================

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using RavenMind.Model;

namespace RavenMind.Tests
{
    [TestClass]
    public class UndoRedoTests
    {
        [TestMethod]
        public void TestReordering()
        {
            Node node1 = new Node();
            Node node2 = new Node();
            Node node3 = new Node();

            Document document = new Document();
            document.Root.LeftChildren.Add(node1);
            document.Root.LeftChildren.Add(node2);
            document.Root.LeftChildren.Add(node3);

            document.StartChangeTracking();
            node1.OrderIndex = 3;
            document.StopChangeTracking();

            Assert.IsTrue(document.UndoRedoManager.CanUndo);
            Assert.AreEqual(1, node2.OrderIndex);
            Assert.AreEqual(2, node3.OrderIndex);
            Assert.AreEqual(3, node1.OrderIndex);

            document.UndoRedoManager.Undo();

            Assert.IsFalse(document.UndoRedoManager.CanUndo);
            Assert.AreEqual(1, node1.OrderIndex);
            Assert.AreEqual(2, node2.OrderIndex);
            Assert.AreEqual(3, node3.OrderIndex);
            Assert.IsTrue(document.UndoRedoManager.CanRedo);

            document.UndoRedoManager.Redo();

            Assert.AreEqual(1, node2.OrderIndex);
            Assert.AreEqual(2, node3.OrderIndex);
            Assert.AreEqual(3, node1.OrderIndex);
        }

        [TestMethod]
        public void TestColorChanged()
        {
            Node node = new Node();
            node.Color = 123;

            Document document = new Document();
            document.Root.LeftChildren.Add(node);

            document.StartChangeTracking();
            node.Color = 456;
            document.StopChangeTracking();

            Assert.IsTrue(document.UndoRedoManager.CanUndo);
            Assert.AreEqual(456, node.Color);

            document.UndoRedoManager.Undo();

            Assert.IsFalse(document.UndoRedoManager.CanUndo);
            Assert.AreEqual(123, node.Color);
            Assert.IsTrue(document.UndoRedoManager.CanRedo);

            document.UndoRedoManager.Redo();

            Assert.AreEqual(456, node.Color);
        }

        [TestMethod]
        public void TestRemove()
        {
            Node node1 = new Node();
            Node node2 = new Node();
            Node node3 = new Node();

            Document document = new Document();
            document.Root.LeftChildren.Add(node1);
            document.Root.LeftChildren.Add(node2);
            document.Root.LeftChildren.Add(node3);

            document.StartChangeTracking();
            document.Root.LeftChildren.Remove(node2);
            document.StopChangeTracking();

            Assert.IsTrue(document.UndoRedoManager.CanUndo);
            Assert.IsTrue(document.Root.LeftChildren.Contains(node1));
            Assert.IsTrue(document.Root.LeftChildren.Contains(node3));
            Assert.AreEqual(1, node1.OrderIndex);
            Assert.AreEqual(2, node3.OrderIndex);

            document.UndoRedoManager.Undo();

            Assert.IsFalse(document.UndoRedoManager.CanUndo);
            Assert.IsTrue(document.Root.LeftChildren.Contains(node1));
            Assert.IsTrue(document.Root.LeftChildren.Contains(node2));
            Assert.IsTrue(document.Root.LeftChildren.Contains(node3));
            Assert.AreEqual(1, node1.OrderIndex);
            Assert.AreEqual(2, node2.OrderIndex);
            Assert.AreEqual(3, node3.OrderIndex);
            Assert.IsTrue(document.UndoRedoManager.CanRedo);

            document.UndoRedoManager.Redo();

            Assert.IsTrue(document.UndoRedoManager.CanUndo);
            Assert.IsTrue(document.Root.LeftChildren.Contains(node1));
            Assert.IsTrue(document.Root.LeftChildren.Contains(node3));
            Assert.AreEqual(1, node1.OrderIndex);
            Assert.AreEqual(2, node3.OrderIndex);
        }

        [TestMethod]
        public void TestNoChanges()
        {
            Document document = new Document();

            document.StartChangeTracking();
            document.StopChangeTracking();

            Assert.IsFalse(document.UndoRedoManager.CanUndo);
        }
    }
}
