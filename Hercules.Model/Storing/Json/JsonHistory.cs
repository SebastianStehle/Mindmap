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
using System.Runtime.Serialization;

namespace Hercules.Model.Storing.Json
{
    [DataContract]
    public sealed class JsonHistory
    {
        [DataMember]
        public Guid Id { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public List<JsonHistoryStep> Steps { get; set; }

        public JsonHistory()
        {
            Steps = new List<JsonHistoryStep>();
        }

        public JsonHistory(Document document)
            : this()
        {
            Id = document.Id;

            foreach (var transaction in document.UndoRedoManager.History.OfType<CompositeUndoRedoAction>())
            {
                JsonHistoryStep jsonStep = CreateJsonStep(transaction);

                Steps.Add(jsonStep);
            }

            Name = document.Title;
        }

        private static JsonHistoryStep CreateJsonStep(CompositeUndoRedoAction transaction)
        {
            JsonHistoryStep jsonStep = new JsonHistoryStep { Name = transaction.Name, Date = transaction.Date };

            foreach (CommandBase command in transaction.Commands)
            {
                JsonHistoryStepCommand jsonCommand = new JsonHistoryStepCommand { CommandType = command.GetType().AssemblyQualifiedName };

                command.Save(jsonCommand.Properties);

                jsonStep.Commands.Add(jsonCommand);
            }

            return jsonStep;
        }

        public Document ToDocument()
        {
            Document document = new Document(Id, Name);

            foreach (JsonHistoryStep step in Steps.Reverse<JsonHistoryStep>())
            {
                document.BeginTransaction(step.Name, step.Date);

                foreach (JsonHistoryStepCommand jsonCommand in step.Commands)
                {
                    Type commandType = Type.GetType(jsonCommand.CommandType);

                    CommandBase command = (CommandBase)Activator.CreateInstance(commandType, jsonCommand.Properties, document);

                    document.Apply(command);
                }

                document.CommitTransaction();
            }

            return document;
        }
    }
}
