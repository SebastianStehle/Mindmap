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

// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
// ReSharper disable MemberCanBePrivate.Global
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
                var jsonStep = CreateJsonStep(transaction);

                Steps.Add(jsonStep);
            }
        }

        private static JsonHistoryStep CreateJsonStep(CompositeUndoRedoAction transaction)
        {
            var jsonStep = new JsonHistoryStep { Name = transaction.Name, Date = transaction.Date };

            foreach (var command in transaction.Actions.OfType<IUndoRedoCommand>())
            {
                var jsonCommand = new JsonHistoryStepCommand { CommandType = CommandFactory.ToTypeName(command) };

                command.Save(jsonCommand.Properties);

                jsonStep.Commands.Add(jsonCommand);
            }

            return jsonStep;
        }

        public Document ToDocument()
        {
            var document = new Document(Id);

            foreach (var step in Steps.Reverse<JsonHistoryStep>())
            {
                document.BeginTransaction(step.Name, step.Date);

                foreach (var jsonCommand in step.Commands)
                {
                    var command = CommandFactory.CreateCommand(jsonCommand.CommandType, jsonCommand.Properties, document);

                    document.Apply(command);
                }

                document.CommitTransaction();
            }

            return document;
        }
    }
}
