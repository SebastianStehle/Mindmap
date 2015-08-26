// ==========================================================================
// UndoRedoManagerTests.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System;
using Hercules.Model;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using UnitTests.Mockups;

namespace UnitTests.Tests
{
    [TestClass]
    public class UndoRedoManagerTests
    {
        private readonly UndoRedoManager undoRedoManager = new UndoRedoManager();

        [TestMethod]
        public void TestConstructor()
        {
            // ReSharper disable once ObjectCreationAsStatement
            new UndoRedoManager();
        }

        [TestMethod]
        public void TestRedoForEmptyLog()
        {
            undoRedoManager.Undo();
        }

        [TestMethod]
        public void TestRedoForSingleItem()
        {
            MockupAction action = new MockupAction();
            
            undoRedoManager.RegisterExecutedAction(action);
            undoRedoManager.UndoAll();
            undoRedoManager.Redo();

            Assert.IsTrue(action.IsRedoInvoked);
            Assert.IsTrue(undoRedoManager.CanUndo);
            Assert.IsFalse(undoRedoManager.CanRedo);
        }

        [TestMethod]
        public void TestRedoAllForMultipleItems()
        {
            MockupAction action1 = new MockupAction();
            MockupAction action2 = new MockupAction();

            undoRedoManager.RegisterExecutedAction(action1);
            undoRedoManager.RegisterExecutedAction(action2);
            undoRedoManager.UndoAll();
            undoRedoManager.RedoAll();

            Assert.IsTrue(action1.IsRedoInvoked);
            Assert.IsTrue(action2.IsRedoInvoked);

            Assert.IsTrue(undoRedoManager.CanUndo);
            Assert.IsFalse(undoRedoManager.CanRedo);
        }

        [TestMethod]
        public void TestUndoForEmptyLog()
        {
            undoRedoManager.Undo();
        }

        [TestMethod]
        public void TestUndoForSingleItem()
        {
            MockupAction action = new MockupAction();
            
            undoRedoManager.RegisterExecutedAction(action);
            undoRedoManager.Undo();

            Assert.IsTrue(action.IsUndoInvoked);

            Assert.IsTrue(undoRedoManager.CanRedo);
            Assert.IsFalse(undoRedoManager.CanUndo);
        }

        [TestMethod]
        public void TestUndoAllForEmptyLog()
        {
            undoRedoManager.UndoAll();
        }

        [TestMethod]
        public void TestUndoAllForSingleItem()
        {
            MockupAction action = new MockupAction();
            
            undoRedoManager.RegisterExecutedAction(action);
            undoRedoManager.UndoAll();

            Assert.IsTrue(action.IsUndoInvoked);

            Assert.IsTrue(undoRedoManager.CanRedo);
            Assert.IsFalse(undoRedoManager.CanUndo);
        }

        [TestMethod]
        public void TestUndoAllForMultipleItems()
        {
            MockupAction action1 = new MockupAction();
            MockupAction action2 = new MockupAction();
            
            undoRedoManager.RegisterExecutedAction(action1);
            undoRedoManager.RegisterExecutedAction(action2);
            undoRedoManager.UndoAll();

            Assert.IsTrue(action1.IsUndoInvoked);
            Assert.IsTrue(action2.IsUndoInvoked);
            Assert.IsTrue(undoRedoManager.CanRedo);
            Assert.IsFalse(undoRedoManager.CanUndo);
        }

        [TestMethod]
        public void TestRegisterExecutedActionNullActionException()
        {
            Assert.ThrowsException<ArgumentNullException>(() => undoRedoManager.RegisterExecutedAction(null));
        }

        [TestMethod]
        public void TestRegisterExecutedAction()
        {
            MockupAction action1 = new MockupAction();
            MockupAction action2 = new MockupAction();

            undoRedoManager.RegisterExecutedAction(action1);

            Assert.IsTrue(undoRedoManager.CanUndo);

            undoRedoManager.RegisterExecutedAction(action2);

            Assert.IsTrue(undoRedoManager.CanUndo);
        }

        [TestMethod]
        public void TestRegisterExecutedActionAfterUndoAll()
        {
            MockupAction oldAction1 = new MockupAction();
            MockupAction oldAction2 = new MockupAction();

            undoRedoManager.RegisterExecutedAction(oldAction1);
            undoRedoManager.RegisterExecutedAction(oldAction2);
            undoRedoManager.UndoAll();

            MockupAction action1 = new MockupAction();
            MockupAction action2 = new MockupAction();

            undoRedoManager.RegisterExecutedAction(action1);

            Assert.IsTrue(undoRedoManager.CanUndo);

            undoRedoManager.RegisterExecutedAction(action2);

            Assert.IsTrue(undoRedoManager.CanUndo);
        }

        [TestMethod]
        public void TestRegisterExecutedActionAfterUndo()
        {
            MockupAction oldAction1 = new MockupAction();
            MockupAction oldAction2 = new MockupAction();

            undoRedoManager.RegisterExecutedAction(oldAction1);
            undoRedoManager.RegisterExecutedAction(oldAction2);
            undoRedoManager.Undo();

            MockupAction action1 = new MockupAction();
            MockupAction action2 = new MockupAction();

            undoRedoManager.RegisterExecutedAction(action1);

            Assert.IsTrue(undoRedoManager.CanUndo);

            undoRedoManager.RegisterExecutedAction(action2);

            Assert.IsTrue(undoRedoManager.CanUndo);
        }
    }
}