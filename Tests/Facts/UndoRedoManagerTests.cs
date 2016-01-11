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
using Tests.Mockups;
using Xunit;

// ReSharper disable once ObjectCreationAsStatement

namespace Tests.Facts
{
    public class UndoRedoManagerTests
    {
        private readonly UndoRedoManager undoRedoManager = new UndoRedoManager();

        [Fact]
        public void Constructor()
        {
            new UndoRedoManager();
        }

        [Fact]
        public void Redo_EmptyLog_DoesNothing()
        {
            undoRedoManager.Redo();
        }

        [Fact]
        public void RedoAll_EmptyLog_DoesNothing()
        {
            undoRedoManager.RedoAll();
        }

        [Fact]
        public void Redo_SingleAction_RedoInvoked()
        {
            MockupAction action = new MockupAction();

            undoRedoManager.RegisterExecutedAction(action);
            undoRedoManager.UndoAll();
            undoRedoManager.Redo();

            Assert.True(action.IsRedoInvoked);
            Assert.True(undoRedoManager.CanUndo);
            Assert.False(undoRedoManager.CanRedo);
        }

        [Fact]
        public void Redo_MultipleItems_RedoInvoked_CanRedo()
        {
            MockupAction action1 = new MockupAction();
            MockupAction action2 = new MockupAction();

            undoRedoManager.RegisterExecutedAction(action1);
            undoRedoManager.RegisterExecutedAction(action2);
            undoRedoManager.UndoAll();
            undoRedoManager.RedoAll();

            Assert.True(action1.IsRedoInvoked);
            Assert.True(action2.IsRedoInvoked);

            Assert.True(undoRedoManager.CanUndo);
            Assert.False(undoRedoManager.CanRedo);
        }

        [Fact]
        public void Undo_EmtyLog_DoesNothing()
        {
            undoRedoManager.Undo();
        }

        [Fact]
        public void UndoAll_EmtyLog_DoesNothing()
        {
            undoRedoManager.UndoAll();
        }

        [Fact]
        public void Undo_SingleAction_UndoInvoked()
        {
            MockupAction action = new MockupAction();

            undoRedoManager.RegisterExecutedAction(action);
            undoRedoManager.Undo();

            Assert.True(action.IsUndoInvoked);

            Assert.True(undoRedoManager.CanRedo);
            Assert.False(undoRedoManager.CanUndo);
        }

        [Fact]
        public void Undo_MultipleActions_UndoInvoked_CanUndo()
        {
            MockupAction action1 = new MockupAction();
            MockupAction action2 = new MockupAction();

            undoRedoManager.RegisterExecutedAction(action1);
            undoRedoManager.RegisterExecutedAction(action2);
            undoRedoManager.UndoAll();

            Assert.True(action1.IsUndoInvoked);
            Assert.True(action2.IsUndoInvoked);
            Assert.True(undoRedoManager.CanRedo);
            Assert.False(undoRedoManager.CanUndo);
        }

        [Fact]
        public void Revert_UndoInvoked_CannotRedo()
        {
            MockupAction action1 = new MockupAction();
            MockupAction action2 = new MockupAction();

            undoRedoManager.RegisterExecutedAction(action1);
            undoRedoManager.RegisterExecutedAction(action2);
            undoRedoManager.Revert();

            Assert.True(action2.IsUndoInvoked);
            Assert.True(undoRedoManager.CanUndo);
            Assert.False(undoRedoManager.CanRedo);
        }

        [Fact]
        public void RevertToIndex_UndoInvoked_CannotRedo()
        {
            MockupAction action1 = new MockupAction();
            MockupAction action2 = new MockupAction();
            MockupAction action3 = new MockupAction();

            undoRedoManager.RegisterExecutedAction(action1);
            undoRedoManager.RegisterExecutedAction(action2);
            undoRedoManager.RegisterExecutedAction(action3);

            Assert.Equal(3, undoRedoManager.Index);

            undoRedoManager.RevertTo(1);

            Assert.Equal(1, undoRedoManager.History.Count());
            Assert.False(undoRedoManager.CanRedo);
            Assert.True(undoRedoManager.CanUndo);
        }

        [Fact]
        public void RegisterExecutedAction_ActionIsNull_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() => undoRedoManager.RegisterExecutedAction(null));
        }

        [Fact]
        public void RegisterExecutedAction_CanUndo()
        {
            MockupAction action1 = new MockupAction();
            MockupAction action2 = new MockupAction();

            undoRedoManager.RegisterExecutedAction(action1);

            Assert.True(undoRedoManager.CanUndo);

            undoRedoManager.RegisterExecutedAction(action2);

            Assert.True(undoRedoManager.CanUndo);
        }

        [Fact]
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

            Assert.True(undoRedoManager.CanUndo);

            undoRedoManager.RegisterExecutedAction(action2);

            Assert.True(undoRedoManager.CanUndo);
        }

        [Fact]
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

            Assert.True(undoRedoManager.CanUndo);

            undoRedoManager.RegisterExecutedAction(action2);

            Assert.True(undoRedoManager.CanUndo);
        }
    }
}