// ==========================================================================
// JsonDocumentStore.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.FileProperties;
using Windows.Storage.Streams;
using GP.Windows;
using Hercules.Model.Storing.Utils;
using Hercules.Model.Utils;
using Newtonsoft.Json;

namespace Hercules.Model.Storing.Json
{
    public sealed class JsonDocumentStore : IDocumentStore
    {
        private const string FileExtension = ".mmd";
        private const string DefaultSubfolder = "Mindapp";
        private readonly JsonSerializerSettings historySerializerSettings = new JsonSerializerSettings();
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

            historySerializerSettings.Converters.Add(new PropertiesBagConverter());
        }

        public Task<IList<DocumentRef>> LoadAllAsync()
        {
            return taskFactory.StartNew(() => LoadAllInternalAsync()).Unwrap();
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

            return taskFactory.StartNew(() => StoreInternalAsync(documentRef, document)).Unwrap();
        }

        public Task<DocumentRef> CreateAsync(string name, Document document)
        {
            Guard.NotNull(document, nameof(document));
            Guard.ValidFileName(name, nameof(name));

            return taskFactory.StartNew(() => CreateInternalAsync(name, document)).Unwrap();
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

        public Task ClearAsync()
        {
            return taskFactory.StartNew(() => ClearInternalAsync()).Unwrap();
        }

        private async Task<IList<DocumentRef>> LoadAllInternalAsync()
        {
            await EnsureFolderAsync();

            List<DocumentRef> documentReferences = new List<DocumentRef>();

            IEnumerable<StorageFile> files = await localFolder.GetFilesAsync();

            foreach (StorageFile file in files.Where(file => file.FileType.Equals(FileExtension, StringComparison.OrdinalIgnoreCase)))
            {
                BasicProperties properties = await file.GetBasicPropertiesAsync();

                documentReferences.Add(new DocumentRef(file.DisplayName, properties.DateModified));
            }

            return documentReferences.OrderByDescending(x => x.LastUpdate).ToList();
        }

        private async Task<Document> LoadInternalAsync(DocumentRef documentRef)
        {
            StorageFile file = await GetFileAsync(documentRef);

            using (IRandomAccessStream stream = await file.OpenReadAsync())
            {
                Stream normalStream = stream.AsStreamForRead();

                JsonHistory history = normalStream.DeserializeAsJsonFromStream<JsonHistory>(historySerializerSettings);

                Document document = history.ToDocument();

                return document;
            }
        }

        private async Task<DocumentRef> CreateInternalAsync(string name, Document document)
        {
            DocumentRef documentRef = new DocumentRef(name, DateTimeOffset.Now);

            StorageFile file = await CreateFileAsync(documentRef, CreationCollisionOption.GenerateUniqueName);

            await file.SerializeAsJsonAsync(new JsonHistory(document), historySerializerSettings);

            return documentRef.Rename(file.DisplayName);
        }

        private async Task RenameInternalAsync(DocumentRef documentRef, string newName)
        {
            StorageFile file = await GetFileAsync(documentRef);

            await file.RenameAsync($"{newName}{FileExtension}", NameCollisionOption.GenerateUniqueName);
            await file.GetBasicPropertiesAsync();

            documentRef.Updated().Rename(file.DisplayName);
        }

        private async Task StoreInternalAsync(DocumentRef documentRef, Document document)
        {
            StorageFile file = await CreateFileAsync(documentRef, CreationCollisionOption.ReplaceExisting);

            await file.SerializeAsJsonAsync(new JsonHistory(document), historySerializerSettings);

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

            string fileName = $"{documentRef.DocumentName}{FileExtension}";

            return await localFolder.CreateFileAsync(fileName, options);
        }

        private async Task<StorageFile> GetFileAsync(DocumentRef documentRef)
        {
            await EnsureFolderAsync();

            string fileName = $"{documentRef.DocumentName}{FileExtension}";

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
