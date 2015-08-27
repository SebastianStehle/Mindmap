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
using Windows.UI.Xaml.Media;
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
        private readonly DocumentRef documentRef;

        public Guid DocumentId
        {
            get
            {
                return documentRef.DocumentId;
            }
        }


        public ImageSource Screenshot
        {
            get
            {
                return documentRef.Screenshot;
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

        public MindmapItem(DocumentRef documentRef)
        {
            Guard.NotNull(documentRef, nameof(documentRef));

            this.documentRef = documentRef;

            Title = documentRef.DocumentTitle;

            LastUpdate = documentRef.LastUpdate;
        }

        public async Task RefreshImageAsync()
        {
            if (documentRef != null)
            {
                await documentRef.LoadImageAsync();

                // ReSharper disable once ExplicitCallerInfoArgument
                RaisePropertyChanged(nameof(Screenshot));
            }
        }

        public void RefreshAfterSave()
        {
            LastUpdate = DateTimeOffset.Now;
        }

        public async Task RemoveAsync(IDocumentStore store)
        {
            Guard.NotNull(store, nameof(store));

            await store.DeleteAsync(documentRef.DocumentId);

            Messenger.Default.Send(new MindmapDeletedMessage(this));
        }
    }
}
