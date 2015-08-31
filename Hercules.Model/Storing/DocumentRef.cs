// ==========================================================================
// DocumentRef.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System;
using GP.Windows;

namespace Hercules.Model.Storing
{
    public sealed class DocumentRef
    {
        private readonly Guid documentId;
        private readonly string documentTitle;
        private readonly DateTimeOffset lastUpdate;

        public Guid DocumentId
        {
            get { return documentId; }
        }

        public string DocumentTitle
        {
            get { return documentTitle; }
        }
       

        public DateTimeOffset LastUpdate
        {
            get { return lastUpdate; }
        }

        public DocumentRef(Guid documentId, string documentTitle, DateTimeOffset lastUpdate)
        {
            Guard.NotNull(documentTitle, nameof(documentTitle));

            this.documentTitle = documentTitle;
            this.documentId = documentId;
            this.lastUpdate = lastUpdate;
        }
    }
}
