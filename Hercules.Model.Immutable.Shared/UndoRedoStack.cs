using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hercules.Model
{
    public class UndoRedoStack<T>
    {
        private readonly Stack<T> undoStack = new Stack<T>();
        private readonly Stack<T> redoStack = new Stack<T>();
        private T current;

        public event EventHandler StateChanged;

        public bool CanUndo
        {
            get { return undoStack.Count > 0; }
        }

        public bool CanRedo
        {
            get { return redoStack.Count > 0; }
        }

        public T Current
        {
            get { return current; }
        }

        public UndoRedoStack(T current)
        {
            this.current = current;
        }

        public void Undo()
        {
            if (!CanUndo)
            {
                return;
            }

            redoStack.Push(current);

            current = undoStack.Pop();

            OnStateChanged();
        }

        public void Redo()
        {
            if (!CanRedo)
            {
                return;
            }

            undoStack.Push(current);

            current = redoStack.Pop();

            OnStateChanged();
        }

        public void Update(T newState, bool addToHistory = true)
        {
            if (addToHistory)
            {
                undoStack.Push(current);
            }

            redoStack.Clear();

            current = newState;

            OnStateChanged();
        }

        protected virtual void OnStateChanged()
        {
            StateChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
