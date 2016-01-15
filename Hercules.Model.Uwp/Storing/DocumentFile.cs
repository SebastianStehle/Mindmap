// ==========================================================================
// DocumentFile.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.AccessCache;
using Windows.Storage.FileProperties;
using Windows.Storage.Streams;
using GP.Utils;
using Hercules.Model.Storing.Json;
// ReSharper disable SuggestBaseTypeForParameter

namespace Hercules.Model.Storing
{
    public class DocumentFile : INotifyPropertyChanged
    {
        private StorageFile file;
        private DateTimeOffset modifiedUtc = DateTimeOffset.UtcNow;
        private Document document;
        private bool hasChanges;

        public event PropertyChangedEventHandler PropertyChanged;

        public string Name
        {
            get { return file?.Name; }
        }

        public string ModifiedUtcText
        {
            get { return ModifiedUtc.ToString("g", CultureInfo.CurrentCulture); }
        }

        public bool HasChanges
        {
            get { return hasChanges; }
        }

        public Document Document
        {
            get { return document; }
        }

        public DateTimeOffset ModifiedUtc
        {
            get { return modifiedUtc; }
        }

        private DocumentFile(StorageFile file, DateTimeOffset modifiedUtc)
        {
            this.file = file;

            this.modifiedUtc = modifiedUtc;
        }

        private DocumentFile(StorageFile file)
        {
            OpenInternal(new Document(Guid.NewGuid()));

            document.Root.ChangeTextTransactional(file.DisplayName);
        }

        public static async Task<DocumentFile> OpenAsync(StorageFile file)
        {
            Guard.NotNull(file, nameof(file));

            return new DocumentFile(file, (await file.GetBasicPropertiesAsync()).DateModified);
        }

        public static DocumentFile CreateNew(StorageFile file)
        {
            Guard.NotNull(file, nameof(file));

            return new DocumentFile(file);
        }

        public static async Task<List<DocumentFile>> QueryRecentFilesAsync()
        {
            List<DocumentFile> files = new List<DocumentFile>();

            AccessListEntryView entries = StorageApplicationPermissions.MostRecentlyUsedList.Entries;

            foreach (AccessListEntry entry in entries)
            {
                StorageFile file = await StorageApplicationPermissions.MostRecentlyUsedList.GetFileAsync(entry.Token);

                if (file != null)
                {
                    files.Add(await OpenAsync(file));
                }
            }

            return files;
        }

        public void AddToRecentList()
        {
            if (file == null)
            {
                return;
            }

            StorageApplicationPermissions.MostRecentlyUsedList.Add(file);
        }

        public async Task RenameAsync(string newName)
        {
            Guard.ValidFileName(newName, nameof(newName));

            if (file == null)
            {
                return;
            }

            await file.RenameAsync(newName + JsonDocumentSerializer.FileExtension, NameCollisionOption.GenerateUniqueName);

            OnPropertyChanged(nameof(Name));
        }

        public async Task OpenAsync()
        {
            if (document != null || file == null)
            {
                return;
            }

            using (IRandomAccessStream stream = await file.OpenReadAsync())
            {
                OpenInternal(JsonDocumentSerializer.Deserialize(stream.AsStreamForRead()));
            }
        }

        public async Task SaveAsAsync(StorageFile newFile)
        {
            Guard.NotNull(newFile, nameof(newFile));

            if (document == null)
            {
                return;
            }

            await SaveAsync(newFile);

            file = newFile;
        }

        public async Task SaveNewAsync()
        {
            if (file == null)
            {
                return;
            }

            OpenInternal(new Document(Guid.NewGuid()));

            await SaveAsync(file);
        }

        public async Task SaveAsync()
        {
            if (document == null || file == null)
            {
                return;
            }

            await SaveAsync(file);
        }

        private async Task SaveAsync(StorageFile targetFile)
        {
            JsonHistory history = new JsonHistory(document);

            using (StorageStreamTransaction transaction = await targetFile.OpenTransactedWriteAsync())
            {
                JsonDocumentSerializer.Serialize(history, transaction.Stream.AsStreamForWrite());

                await transaction.CommitAsync();
            }

            modifiedUtc = DateTime.UtcNow;

            hasChanges = true;

            OnPropertyChanged(nameof(ModifiedUtcText));
            OnPropertyChanged(nameof(HasChanges));
        }

        public bool CanRenameTo(string newName)
        {
            return newName.IsValidFileName();
        }

        public void Close()
        {
            CloseInternal();
        }

        private void OpenInternal(Document newDocument)
        {
            document = newDocument;

            if (document != null)
            {
                document.StateChanged += Document_StateChanged;
            }
        }

        private void CloseInternal()
        {
            if (document != null)
            {
                document.StateChanged -= Document_StateChanged;
            }

            document = null;
        }

        private void Document_StateChanged(object sender, EventArgs e)
        {
            hasChanges = true;

            OnPropertyChanged(nameof(HasChanges));
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
