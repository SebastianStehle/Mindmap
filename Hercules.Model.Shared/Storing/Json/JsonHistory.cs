// ==========================================================================
// JsonHistory.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System;
using System.Collections.Generic;
using System.Linq;
using GP.Utils;
using Newtonsoft.Json;

// ReSharper disable LoopCanBePartlyConvertedToQuery

namespace Hercules.Model.Storing.Json
{
    public sealed class JsonHistory
    {
        [JsonProperty]
        public Guid Id { get; set; }

        [JsonProperty]
        public List<JsonHistoryStep> Steps { get; set; }

        public JsonHistory()
        {
            Steps = new List<JsonHistoryStep>();
        }

        public JsonHistory(Document document)
            : this()
        {
            Id = document.Root.Id;

            foreach (var transaction in document.UndoRedoManager.History.OfType<CompositeUndoRedoAction>())
            {
                JsonHistoryStep jsonStep = CreateJsonStep(transaction);

                Steps.Add(jsonStep);
            }
        }

        private static JsonHistoryStep CreateJsonStep(CompositeUndoRedoAction transaction)
        {
            JsonHistoryStep jsonStep = new JsonHistoryStep { Name = transaction.Name, Date = transaction.Date };

            foreach (IUndoRedoCommand command in transaction.Actions.OfType<IUndoRedoCommand>())
            {
                JsonHistoryStepCommand jsonCommand = new JsonHistoryStepCommand { CommandType = CommandFactory.ToTypeName(command) };

                command.Save(jsonCommand.Properties);

                jsonStep.Commands.Add(jsonCommand);
            }

            return jsonStep;
        }

        public Document ToDocument()
        {
            Document document = new Document(Id);

            foreach (JsonHistoryStep step in Steps.Reverse<JsonHistoryStep>())
            {
                document.BeginTransaction(step.Name, step.Date);

                foreach (JsonHistoryStepCommand jsonCommand in step.Commands)
                {
                    IUndoRedoCommand command = CommandFactory.CreateCommand(jsonCommand.CommandType, jsonCommand.Properties, document);

                    document.Apply(command);
                }

                document.CommitTransaction();
            }

            return document;
        }
    }
}
