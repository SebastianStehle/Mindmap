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
using Hercules.Model.Storing;
using Windows.UI.Xaml.Media;

namespace Hercules.App.ViewModels
{
    [ImplementPropertyChanged]
    public sealed class MindmapItem : ViewModelBase
    {
        [NotifyUI]
        public string Title { get; set; }

        [NotifyUI]
        public Guid DocumentId { get; set; }

        [NotifyUI]
        public ImageSource Screenshot { get; set; }

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
            Title = documentRef.DocumentTitle;

            DocumentId = documentRef.DocumentId;

            LastUpdate = documentRef.LastUpdate;

            Screenshot = documentRef.Screenshot;
        }
    }
}
