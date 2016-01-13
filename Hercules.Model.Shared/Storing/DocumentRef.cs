// ==========================================================================
// DocumentRef.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System;
using GP.Utils;

namespace Hercules.Model.Storing
{
    public sealed class DocumentRef
    {
        private DateTimeOffset lastUpdate;
        private string documentName;

        public string DocumentName
        {
            get { return documentName; }
        }

        public DateTimeOffset LastUpdate
        {
            get { return lastUpdate; }
        }

        public DocumentRef(string documentName, DateTimeOffset lastUpdate)
        {
            Guard.ValidFileName(documentName, nameof(documentName));

            this.documentName = documentName;
            this.lastUpdate = lastUpdate;
        }

        public DocumentRef Rename(string name)
        {
            Guard.NotNullOrEmpty(name, nameof(name));

            documentName = name;

            return this;
        }

        public DocumentRef Updated()
        {
            lastUpdate = DateTimeOffset.Now;

            return this;
        }
    }
}
