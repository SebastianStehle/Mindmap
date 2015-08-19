// ==========================================================================
// MindmapItem.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================
using GalaSoft.MvvmLight;
using GP.Windows;
using PropertyChanged;
using System;
using System.Globalization;
using System.Threading.Tasks;
using Hercules.Model.Storing;
using Windows.UI.Xaml.Media;

namespace Hercules.App.ViewModels
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

                RaisePropertyChanged(nameof(Screenshot));
            }
        }
    }
}
