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

namespace Hercules.App.Modules
{
    [ImplementPropertyChanged]
    public sealed class MindmapItem : ViewModelBase
    {
        private readonly IDocumentStore documentStore;
        private readonly DocumentRef documentRef;

        public Guid DocumentId
        {
            get
            {
                return documentRef.DocumentId;
            }
        }

        [NotifyUI]
        public string Title { get; set; }

        [NotifyUI]
        public DateTimeOffset LastUpdate { get; set; }

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

            Title = documentRef.DocumentTitle;

            LastUpdate = documentRef.LastUpdate;
        }

        public void RefreshAfterSave()
        {
            LastUpdate = DateTimeOffset.Now;
        }

        public async Task RenameAsync(string title)
        {
            Guard.NotNullOrEmpty(title, nameof(title));

            if (documentRef != null)
            {
                Title = title.Trim();

                await documentStore.RenameAsync(documentRef.DocumentId, title);
            }
        }

        public async Task RemoveAsync()
        {
            if (documentRef != null)
            {
                await documentStore.DeleteAsync(documentRef.DocumentId);

                Messenger.Default.Send(new MindmapDeletedMessage(this));
            }
        }
    }
}
