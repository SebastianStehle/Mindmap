// ==========================================================================
// RootNode.cs
// Hercules Application
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using GP.Windows;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Hercules.Model
{
    public sealed class CompositeUndoRedoAction : IUndoRedoAction
    {
        private readonly List<CommandBase> commands = new List<CommandBase>();
        private readonly DateTimeOffset date;
        private readonly string name;

        public DateTimeOffset Date
        {
            get
            {
                return date;
            }
        }

        public string Name
        {
            get
            {
                return name;
            }
        }

        public IReadOnlyList<CommandBase> Commands
        {
            get
            {
                return commands;
            }
        }

        public CompositeUndoRedoAction(string name, DateTimeOffset date)
        {
            Guard.NotNullOrEmpty(name, nameof(name));
          
            this.date = date;
            this.name = name;
        }

        public void Add(CommandBase command)
        {
            Guard.NotNull(command, nameof(command));

            commands.Add(command);
        }

        public void Undo()
        {
            foreach (IUndoRedoAction action in commands.OfType<IUndoRedoAction>().Reverse())
            {
                if (action != null)
                {
                    action.Undo();
                }
            }
        }

        public void Redo()
        {
            foreach (IUndoRedoAction action in commands)
            {
                if (action != null)
                {
                    action.Redo();
                }
            }
        }
    }
}
