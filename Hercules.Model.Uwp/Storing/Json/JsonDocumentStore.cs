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
using Windows.System.Threading;
using GP.Utils;
using Hercules.Model.Storing.Utils;

namespace Hercules.Model.Storing.Json
{
    public sealed class JsonDocumentStore : IDocumentStore
    {
        private const string DefaultSubfolder = "Mindapp";
        private readonly TaskFactory taskFactory = new TaskFactory(new LimitedConcurrencyLevelTaskScheduler(1, x => ThreadPool.RunAsync(y => x(), WorkItemPriority.Normal).Forget()));
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

        public Task<List<DocumentRef>> LoadAllAsync()
        {
            return taskFactory.StartNew(async () =>
            {
                await EnsureFolderAsync();

                List<StorageFile> files = await localFolder.GetFilesAsync(JsonDocumentSerializer.FileExtension);

                List<DocumentRef> result = new List<DocumentRef>();

                foreach (StorageFile file in files)
                {
                    BasicProperties properties = await file.GetBasicPropertiesAsync();

                    result.Add(new DocumentRef(file.NameWithoutExtension(JsonDocumentSerializer.FileExtension), properties.DateModified));
                }

                return result.OrderByDescending(x => x.LastUpdate).ToList();
            }).Unwrap();
        }

        public Task<Document> LoadAsync(DocumentRef documentRef)
        {
            Guard.NotNull(documentRef, nameof(documentRef));

            return taskFactory.StartNew(async () =>
            {
                try
                {
                    StorageFile file = await GetFileAsync(documentRef);

                    return await JsonDocumentFileSerializer.DeserializeFromFileAsync(file);
                }
                catch (FileNotFoundException e)
                {
                    throw new DocumentNotFoundException($"Cannot find the document '{documentRef.DocumentName}'", e);
                }
            }).Unwrap();
        }

        public Task StoreAsync(DocumentRef documentRef, Document document)
        {
            Guard.NotNull(document, nameof(document));
            Guard.NotNull(documentRef, nameof(documentRef));

            JsonHistory history = new JsonHistory(document);

            return taskFactory.StartNew(async () =>
            {
                try
                {
                    StorageFile file = await GetOrCreateFileAsync(documentRef);

                    await JsonDocumentFileSerializer.SerializeToFileAsync(file, history);

                    documentRef.Updated().Rename(file.NameWithoutExtension(JsonDocumentSerializer.FileExtension));
                }
                catch (FileNotFoundException e)
                {
                    throw new DocumentNotFoundException($"Cannot find the document '{documentRef.DocumentName}'", e);
                }
            }).Unwrap();
        }

        public Task RenameAsync(DocumentRef documentRef, string newName)
        {
            Guard.NotNull(documentRef, nameof(documentRef));
            Guard.ValidFileName(newName, nameof(newName));

            return taskFactory.StartNew(async () =>
            {
                newName = newName.Trim();
                try
                {
                    StorageFile file = await GetFileAsync(documentRef);

                    await file.RenameAsync($"{newName}{JsonDocumentSerializer.FileExtension.Extension}", NameCollisionOption.GenerateUniqueName);

                    documentRef.Updated().Rename(file.NameWithoutExtension(JsonDocumentSerializer.FileExtension));
                }
                catch (FileNotFoundException e)
                {
                    throw new DocumentNotFoundException($"Cannot find the document '{documentRef.DocumentName}'", e);
                }
            }).Unwrap();
        }

        public Task<bool> DeleteAsync(DocumentRef documentRef)
        {
            Guard.NotNull(documentRef, nameof(documentRef));

            return taskFactory.StartNew(async () =>
            {
                try
                {
                    StorageFile file = await GetFileAsync(documentRef);

                    await file.DeleteAsync();

                    return true;
                }
                catch (FileNotFoundException)
                {
                    return false;
                }
            }).Unwrap();
        }

        public Task<DocumentRef> CreateAsync(string name, Document document)
        {
            Guard.NotNull(document, nameof(document));
            Guard.ValidFileName(name, nameof(name));

            JsonHistory history = new JsonHistory(document);

            return taskFactory.StartNew(async () =>
            {
                DocumentRef documentRef = new DocumentRef(name, DateTimeOffset.Now);

                try
                {
                    StorageFile file = await CreateFileAsync(documentRef);

                    await JsonDocumentFileSerializer.SerializeToFileAsync(file, history);

                    documentRef.Updated().Rename(file.NameWithoutExtension(JsonDocumentSerializer.FileExtension));

                    return documentRef;
                }
                catch (FileNotFoundException e)
                {
                    throw new DocumentNotFoundException($"Cannot find the document '{documentRef.DocumentName}'", e);
                }
            }).Unwrap();
        }

        private async Task EnsureFolderAsync()
        {
            if (localFolder == null)
            {
                localFolder = await ApplicationData.Current.LocalFolder.GetOrCreateFolderAsync(subfolderName);
            }
        }

        private async Task<StorageFile> CreateFileAsync(DocumentRef documentRef)
        {
            await EnsureFolderAsync();

            string fileName = $"{documentRef.DocumentName}{JsonDocumentSerializer.FileExtension.Extension}";

            return await localFolder.CreateFileAsync(fileName, CreationCollisionOption.GenerateUniqueName);
        }

        private async Task<StorageFile> GetOrCreateFileAsync(DocumentRef documentRef)
        {
            await EnsureFolderAsync();

            string fileName = $"{documentRef.DocumentName}{JsonDocumentSerializer.FileExtension.Extension}";

            return await localFolder.GetOrCreateFileAsync(fileName);
        }

        private async Task<StorageFile> GetFileAsync(DocumentRef documentRef)
        {
            await EnsureFolderAsync();

            string fileName = $"{documentRef.DocumentName}{JsonDocumentSerializer.FileExtension.Extension}";

            return await localFolder.GetFileAsync(fileName);
        }
    }
}
