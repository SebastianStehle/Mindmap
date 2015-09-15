// ==========================================================================
// MindmapItem.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System;
using System.Globalization;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using GP.Windows;
using Hercules.App.Messages;
using Hercules.Model.Storing;
using PropertyChanged;
// ReSharper disable ExplicitCallerInfoArgument

namespace Hercules.App.Modules
{
    [ImplementPropertyChanged]
    public sealed class MindmapItem : ViewModelBase
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

        public MindmapItem()
        {
        }

        public MindmapItem(DocumentRef documentRef, IDocumentStore documentStore)
        {
            Guard.NotNull(documentRef, nameof(documentRef));
            Guard.NotNull(documentStore, nameof(documentStore));

            this.documentRef = documentRef;
            this.documentStore = documentStore;
        }

        public void RefreshAfterSave()
        {
            RaisePropertyChanged(nameof(LastUpdate));
            RaisePropertyChanged(nameof(LastUpdateText));
        }

        public async Task RenameAsync(string title)
        {
            Guard.NotNullOrEmpty(title, nameof(title));

            if (documentRef != null)
            {
                await documentStore.RenameAsync(documentRef, title);

                RaisePropertyChanged(nameof(Title));
                RefreshAfterSave();
            }
        }

        public async Task RemoveAsync()
        {
            if (documentRef != null)
            {
                await documentStore.DeleteAsync(documentRef);

                Messenger.Default.Send(new MindmapDeletedMessage(this));
            }
        }
    }
}
