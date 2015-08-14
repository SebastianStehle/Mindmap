// ==========================================================================
// DocumentRef.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================
using GP.Windows;
using System;

namespace Hercules.Model.Storing
{
    public sealed class DocumentRef
    {
        private readonly Guid documentId;
        private readonly string documentName;
        private readonly DateTimeOffset lastUpdate;
        
        public Guid DocumentId
        {
            get
            {
                return documentId;
            }
        }

        public string DocumentName
        {
            get
            {
                return documentName;
            }
        }

        public DateTimeOffset LastUpdate
        {
            get
            {
                return lastUpdate;
            }
        }

        public DocumentRef(Guid documentId, string documentName, DateTimeOffset lastUpdate)
        {
            Guard.NotNull(documentName, nameof(documentName));

            this.documentName = documentName;
            this.documentId = documentId;
            this.lastUpdate = lastUpdate;
        }
    }
}
