// ==========================================================================
// MindmapRef.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System;
using System.Globalization;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GP.Windows;
using Hercules.Model;
using Hercules.Model.Storing;

namespace Hercules.App.Components.Implementations
{
    public sealed class MindmapRef : ViewModelBase, IMindmapRef
    {
        private readonly IDocumentStore documentStore;
        private readonly DocumentRef documentRef;
        
        public string Title
        {
            get { return documentRef.DocumentName; }
        }
        
        public DateTimeOffset LastUpdate
        {
            get { return documentRef.LastUpdate; }
        }

        public DocumentRef DocumentRef
        {
            get { return documentRef; }
        }

        public string LastUpdateText
        {
            get
            {
                return LastUpdate.ToString("g", CultureInfo.CurrentCulture);
            }
        }

        public MindmapRef()
        {
        }

        public MindmapRef(DocumentRef documentRef, IDocumentStore documentStore)
        {
            Guard.NotNull(documentRef, nameof(documentRef));
            Guard.NotNull(documentStore, nameof(documentStore));

            this.documentRef = documentRef;
            this.documentStore = documentStore;
        }

        internal async Task DeleteAsync()
        {
            if (documentRef != null)
            {
                await documentStore.DeleteAsync(documentRef);
            }
        }

        internal async Task SaveAsync(Document document)
        {
            if (documentRef != null)
            {
                await documentStore.StoreAsync(documentRef, document);

                RefreshProperties();
            }
        }

        public async Task RenameAsync(string newTitle)
        {
            if (documentRef != null)
            {
                await documentStore.RenameAsync(documentRef, newTitle);

                RefreshProperties();
            }
        }

        public void RefreshProperties()
        {
            RaisePropertyChanged(nameof(Title));
            RaisePropertyChanged(nameof(LastUpdate));
            RaisePropertyChanged(nameof(LastUpdateText));
        }
    }
}
