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
using Newtonsoft.Json;

// ReSharper disable LoopCanBePartlyConvertedToQuery

namespace Hercules.Model.Storing.Json
{
    public sealed class JsonHistory
    {
        [JsonProperty]
        public Guid Id { get; set; }

        [JsonProperty]
        public List<IAction> Actions { get; set; }

        public JsonHistory()
        {
        }

        public JsonHistory(Document document)
        {
            Id = document.Root().Id;

            Actions = document.UndoRedoStack.History.Select(s => s.SourceAction).ToList();
        }

        public Document ToDocument()
        {
            Document document = new Document(Id, string.Empty);

            foreach (IAction action in Actions)
            {
                document.Dispatch(action);
            }

            return document;
        }
    }
}
