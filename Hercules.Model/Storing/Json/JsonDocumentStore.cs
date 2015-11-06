// ==========================================================================
// JsonDocumentStore.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.FileProperties;
using GP.Windows;
using Hercules.Model.Storing.Utils;
using Hercules.Model.Utils;

namespace Hercules.Model.Storing.Json
{
    public sealed class JsonDocumentStore : IDocumentStore
    {
        private const string DefaultSubfolder = "Mindapp";
        private readonly TaskFactory taskFactory = new TaskFactory(new LimitedThreadsScheduler());
        private readonly string subfolderName;
        private StorageFolder localFolder;

        public JsonDocumentStore()
            : this(DefaultSubfolder)
        {
        }

        private JsonDocumentStore(string subfolderName)
        {
            Guard.NotNullOrEmpty(subfolderName, nameof(subfolderName));

            this.subfolderName = subfolderName;
        }

        public Task<IList<DocumentRef>> LoadAllAsync()
        {
            return taskFactory.StartNew(() => LoadAllInternalAsync()).Unwrap();
        }

        public Task ClearAsync()
        {
            return taskFactory.StartNew(() => ClearInternalAsync()).Unwrap();
        }

        public Task RenameAsync(DocumentRef documentRef, string newName)
        {
            Guard.NotNull(documentRef, nameof(documentRef));
            Guard.ValidFileName(newName, nameof(newName));

            return taskFactory.StartNew(() => RenameInternalAsync(documentRef, newName)).Unwrap();
        }

        public Task StoreAsync(DocumentRef documentRef, Document document)
        {
            Guard.NotNull(document, nameof(document));
            Guard.NotNull(documentRef, nameof(documentRef));

            JsonHistory history = new JsonHistory(document);

            return taskFactory.StartNew(() => StoreInternalAsync(documentRef, history)).Unwrap();
        }

        public Task<DocumentRef> CreateAsync(string name, Document document)
        {
            Guard.NotNull(document, nameof(document));
            Guard.ValidFileName(name, nameof(name));

            JsonHistory history = new JsonHistory(document);

            return taskFactory.StartNew(() => CreateInternalAsync(name, history)).Unwrap();
        }

        public Task<Document> LoadAsync(DocumentRef documentRef)
        {
            Guard.NotNull(documentRef, nameof(documentRef));

            return taskFactory.StartNew(() => LoadInternalAsync(documentRef)).Unwrap();
        }

        public Task DeleteAsync(DocumentRef documentRef)
        {
            Guard.NotNull(documentRef, nameof(documentRef));

            return taskFactory.StartNew(() => DeleteInternalAsync(documentRef)).Unwrap();
        }

        private async Task<IList<DocumentRef>> LoadAllInternalAsync()
        {
            await EnsureFolderAsync();

            List<DocumentRef> documentReferences = new List<DocumentRef>();

            foreach (StorageFile file in await localFolder.GetFilesAsync(JsonDocumentSerializer.FileExtension))
            {
                BasicProperties properties = await file.GetBasicPropertiesAsync();

                documentReferences.Add(new DocumentRef(file.DisplayName, properties.DateModified));
            }

            return documentReferences.OrderByDescending(x => x.LastUpdate).ToList();
        }

        private async Task<Document> LoadInternalAsync(DocumentRef documentRef)
        {
            StorageFile file = await GetFileAsync(documentRef);

            return await JsonDocumentSerializer.DeserializeFromFileAsync(file);
        }

        private async Task<DocumentRef> CreateInternalAsync(string name, JsonHistory history)
        {
            DocumentRef documentRef = new DocumentRef(name, DateTimeOffset.Now);

            StorageFile file = await CreateFileAsync(documentRef, CreationCollisionOption.GenerateUniqueName);

            await JsonDocumentSerializer.SerializeToFileAsync(file, history);

            return documentRef.Rename(file.DisplayName);
        }

        private async Task RenameInternalAsync(DocumentRef documentRef, string newName)
        {
            StorageFile file = await GetFileAsync(documentRef);

            await file.RenameAsync($"{newName}{JsonDocumentSerializer.FileExtension.Extension}", NameCollisionOption.GenerateUniqueName);
            await file.GetBasicPropertiesAsync();

            documentRef.Updated().Rename(file.DisplayName);
        }

        private async Task StoreInternalAsync(DocumentRef documentRef, JsonHistory history)
        {
            StorageFile file = await CreateFileAsync(documentRef, CreationCollisionOption.ReplaceExisting);

            await JsonDocumentSerializer.SerializeToFileAsync(file, history);

            documentRef.Updated();
        }

        private async Task DeleteInternalAsync(DocumentRef documentRef)
        {
            StorageFile file = await GetFileAsync(documentRef);

            await file.DeleteAsync();
        }

        private async Task<StorageFile> CreateFileAsync(DocumentRef documentRef, CreationCollisionOption options)
        {
            await EnsureFolderAsync();

            string fileName = $"{documentRef.DocumentName}{JsonDocumentSerializer.FileExtension.Extension}";

            return await localFolder.CreateFileAsync(fileName, options);
        }

        private async Task<StorageFile> GetFileAsync(DocumentRef documentRef)
        {
            await EnsureFolderAsync();

            string fileName = $"{documentRef.DocumentName}{JsonDocumentSerializer.FileExtension.Extension}";

            return await localFolder.GetFileAsync(fileName);
        }

        private async Task ClearInternalAsync()
        {
            await EnsureFolderAsync();

            await localFolder.DeleteAsync();
        }

        private async Task EnsureFolderAsync()
        {
            if (localFolder == null)
            {
                localFolder = await ApplicationData.Current.LocalFolder.GetOrCreateFolderAsync(subfolderName);
            }
        }
    }
}
