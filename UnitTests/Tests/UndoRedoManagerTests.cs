// ==========================================================================
// UndoRedoManagerTests.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System;
using System.Linq;
using Hercules.Model;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using UnitTests.Mockups;

// ReSharper disable once ObjectCreationAsStatement

namespace UnitTests.Tests
{
    [TestClass]
    public class UndoRedoManagerTests
    {
        private readonly UndoRedoManager undoRedoManager = new UndoRedoManager();

        [TestMethod]
        public void Constructor()
        {
            new UndoRedoManager();
        }

        [TestMethod]
        public void Redo_EmptyLog_DoesNothing()
        {
            undoRedoManager.Redo();
        }

        [TestMethod]
        public void RedoAll_EmptyLog_DoesNothing()
        {
            undoRedoManager.RedoAll();
        }

        [TestMethod]
        public void Redo_SingleAction_RedoInvoked()
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
        public void Redo_MultipleItems_RedoInvoked_CanRedo()
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
        public void Undo_EmtyLog_DoesNothing()
        {
            undoRedoManager.Undo();
        }

        [TestMethod]
        public void UndoAll_EmtyLog_DoesNothing()
        {
            undoRedoManager.UndoAll();
        }

        [TestMethod]
        public void Undo_SingleAction_UndoInvoked()
        {
            MockupAction action = new MockupAction();

            undoRedoManager.RegisterExecutedAction(action);
            undoRedoManager.Undo();

            Assert.IsTrue(action.IsUndoInvoked);

            Assert.IsTrue(undoRedoManager.CanRedo);
            Assert.IsFalse(undoRedoManager.CanUndo);
        }

        [TestMethod]
        public void Undo_MultipleActions_UndoInvoked_CanUndo()
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
        public void Revert_UndoInvoked_CannotRedo()
        {
            MockupAction action1 = new MockupAction();
            MockupAction action2 = new MockupAction();

            undoRedoManager.RegisterExecutedAction(action1);
            undoRedoManager.RegisterExecutedAction(action2);
            undoRedoManager.Revert();

            Assert.IsTrue(action2.IsUndoInvoked);
            Assert.IsTrue(undoRedoManager.CanUndo);
            Assert.IsFalse(undoRedoManager.CanRedo);
        }

        [TestMethod]
        public void RevertToIndex_UndoInvoked_CannotRedo()
        {
            MockupAction action1 = new MockupAction();
            MockupAction action2 = new MockupAction();
            MockupAction action3 = new MockupAction();

            undoRedoManager.RegisterExecutedAction(action1);
            undoRedoManager.RegisterExecutedAction(action2);
            undoRedoManager.RegisterExecutedAction(action3);

            Assert.AreEqual(3, undoRedoManager.Index);

            undoRedoManager.RevertTo(1);

            Assert.AreEqual(1, undoRedoManager.History.Count());
            Assert.IsFalse(undoRedoManager.CanRedo);
            Assert.IsTrue(undoRedoManager.CanUndo);
        }

        [TestMethod]
        public void RegisterExecutedAction_ActionIsNull_ThrowsException()
        {
            Assert.ThrowsException<ArgumentNullException>(() => undoRedoManager.RegisterExecutedAction(null));
        }

        [TestMethod]
        public void RegisterExecutedAction_CanUndo()
        {
            MockupAction action1 = new MockupAction();
            MockupAction action2 = new MockupAction();

            undoRedoManager.RegisterExecutedAction(action1);

            Assert.IsTrue(undoRedoManager.CanUndo);

            undoRedoManager.RegisterExecutedAction(action2);

            Assert.IsTrue(undoRedoManager.CanUndo);
        }

        [TestMethod]
        public void RegisterExecutedAction_WithUndoneActions_StartedFromScratch()
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
        public void RegisterExecutedAction_WithUndoAction_StartedAfterFirstAction()
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