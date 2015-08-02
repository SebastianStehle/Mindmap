// ==========================================================================
// UndoRedoManagerTests.cs
// SD Components
// ==========================================================================
// Copyright (c) Silverlight Shapes Development Group
// All rights reserved.
// ==========================================================================

using System;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using RavenMind.Mockups;
using RavenMind.Model;

namespace RavenMind.Tests
{
    [TestClass]
    public class UndoRedoManagerTests
    {
        [TestMethod]
        public void TestRedoForEmptyLog()
        {
            UndoRedoManager undoRedoManager = new UndoRedoManager();
            undoRedoManager.Undo();
        }

        [TestMethod]
        public void TestRedoForSingleItem()
        {
            MockupAction action = new MockupAction();

            UndoRedoManager undoRedoManager = new UndoRedoManager();
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

            UndoRedoManager undoRedoManager = new UndoRedoManager();
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
            UndoRedoManager undoRedoManager = new UndoRedoManager();
            undoRedoManager.Undo();
        }

        [TestMethod]
        public void TestUndoForSingleItem()
        {
            MockupAction action = new MockupAction();

            UndoRedoManager undoRedoManager = new UndoRedoManager();
            undoRedoManager.RegisterExecutedAction(action);
            undoRedoManager.Undo();

            Assert.IsTrue(action.IsUndoInvoked);

            Assert.IsTrue(undoRedoManager.CanRedo);
            Assert.IsFalse(undoRedoManager.CanUndo);
        }

        [TestMethod]
        public void TestUndoAllForEmptyLog()
        {
            UndoRedoManager undoRedoManager = new UndoRedoManager();
            undoRedoManager.UndoAll();
        }

        [TestMethod]
        public void TestUndoAllForSingleItem()
        {
            MockupAction action = new MockupAction();

            UndoRedoManager undoRedoManager = new UndoRedoManager();
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

            UndoRedoManager undoRedoManager = new UndoRedoManager();
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
            UndoRedoManager undoRedoManager = new UndoRedoManager();
            
            Assert.ThrowsException<ArgumentNullException>(() => undoRedoManager.RegisterExecutedAction(null));
        }

        [TestMethod]
        public void TestRegisterExecutedAction()
        {
            UndoRedoManager undoRedoManager = new UndoRedoManager();

            MockupAction action1 = new MockupAction();
            action1.Description = "Action1";

            MockupAction action2 = new MockupAction();
            action2.Description = "Action2";

            undoRedoManager.RegisterExecutedAction(action1);

            Assert.IsTrue(undoRedoManager.CanUndo);
            undoRedoManager.RegisterExecutedAction(action2);
            Assert.IsTrue(undoRedoManager.CanUndo);
        }

        [TestMethod]
        public void TestRegisterExecutedActionAfterUndoAll()
        {
            UndoRedoManager undoRedoManager = new UndoRedoManager();

            MockupAction oldAction1 = new MockupAction();
            MockupAction oldAction2 = new MockupAction();

            undoRedoManager.RegisterExecutedAction(oldAction1);
            undoRedoManager.RegisterExecutedAction(oldAction2);
            undoRedoManager.UndoAll();

            MockupAction action1 = new MockupAction();
            action1.Description = "Action1";

            MockupAction action2 = new MockupAction();
            action2.Description = "Action2";

            undoRedoManager.RegisterExecutedAction(action1);

            Assert.IsTrue(undoRedoManager.CanUndo);
            undoRedoManager.RegisterExecutedAction(action2);
            Assert.IsTrue(undoRedoManager.CanUndo);
        }

        [TestMethod]
        public void TestRegisterExecutedActionAfterUndo()
        {
            UndoRedoManager undoRedoManager = new UndoRedoManager();

            MockupAction oldAction1 = new MockupAction();
            oldAction1.Description = "Action1";

            MockupAction oldAction2 = new MockupAction();
            oldAction2.Description = "Action2";

            undoRedoManager.RegisterExecutedAction(oldAction1);
            undoRedoManager.RegisterExecutedAction(oldAction2);
            undoRedoManager.Undo();

            MockupAction action1 = new MockupAction();
            action1.Description = "Action3";

            MockupAction action2 = new MockupAction();
            action2.Description = "Action4";

            undoRedoManager.RegisterExecutedAction(action1);

            Assert.IsTrue(undoRedoManager.CanUndo);
            undoRedoManager.RegisterExecutedAction(action2);
            Assert.IsTrue(undoRedoManager.CanUndo);
        }

        [TestMethod]
        public void TestConstructor()
        {
            UndoRedoManager undoRedoManager = new UndoRedoManager();
        }
    }
}