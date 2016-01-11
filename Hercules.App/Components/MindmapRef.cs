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
using GP.Utils;
using Hercules.Model;
using Hercules.Model.Storing;

namespace Hercules.App.Components
{
    public sealed class MindmapRef : ViewModelBase
    {
        private readonly IMindmapStore mindmapStore;
        private readonly DocumentRef documentRef;

        public string Name
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

        public MindmapRef(DocumentRef documentRef, IMindmapStore mindmapStore)
        {
            Guard.NotNull(documentRef, nameof(documentRef));
            Guard.NotNull(mindmapStore, nameof(mindmapStore));

            this.documentRef = documentRef;
            this.mindmapStore = mindmapStore;
        }

        public bool CanRenameTo(string newName)
        {
            return mindmapStore != null && mindmapStore.IsValidMindmapName(newName);
        }

        public async Task DeleteAsync()
        {
            if (documentRef != null)
            {
                await mindmapStore.DeleteAsync(this);
            }
        }

        public async Task SaveAsync(Document document)
        {
            if (documentRef != null)
            {
                await mindmapStore.SaveAsync(this, document);
            }
        }

        public async Task RenameAsync(string newName)
        {
            Guard.ValidFileName(newName, nameof(newName));

            if (documentRef != null)
            {
                await mindmapStore.RenameAsync(this, newName);
            }
        }

        public void RefreshProperties()
        {
            RaisePropertyChanged(nameof(Name));
            RaisePropertyChanged(nameof(LastUpdate));
            RaisePropertyChanged(nameof(LastUpdateText));
        }
    }
}
