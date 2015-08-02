// ==========================================================================
// MindmapItem.cs
// Mindmap Application
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using GalaSoft.MvvmLight;
using GP.Windows;
using PropertyChanged;
using System;
using Hercules.Model.Storing;
using System.Globalization;

namespace Hercules.App.ViewModels
{
    [ImplementPropertyChanged]
    public sealed class MindmapItem : ViewModelBase
    {
        [NotifyUI]
        public Guid MindmapId { get; set; }

        [NotifyUI]
        public string Name { get; set; }

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
            Name = documentRef.DocumentName;

            MindmapId = documentRef.DocumentId;

            LastUpdate = documentRef.LastUpdate;
        }
    }
}
