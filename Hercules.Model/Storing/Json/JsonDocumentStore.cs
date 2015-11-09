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
            return taskFactory.StartNew(LoadAllInternalAsync).Unwrap();
        }

        private async Task<IList<DocumentRef>> LoadAllInternalAsync()
        {
            await EnsureFolderAsync();

            List<StorageFile> files = await localFolder.GetFilesAsync(JsonDocumentSerializer.FileExtension);

            List<Task<BasicProperties>> propertiesTasks = files.Select(file => file.GetBasicPropertiesAsync().AsTask()).ToList();

            await Task.WhenAll(propertiesTasks);

            return propertiesTasks.Select((t, i) => new DocumentRef(files[i].DisplayName, t.Result.DateModified)).OrderByDescending(x => x.LastUpdate).ToList();
        }

        public Task<Document> LoadAsync(DocumentRef documentRef)
        {
            Guard.NotNull(documentRef, nameof(documentRef));

            return taskFactory.StartNew(async () =>
            {
                try
                {
                    StorageFile file = await GetFileAsync(documentRef);

                    return await JsonDocumentSerializer.DeserializeFromFileAsync(file);
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
                    StorageFile file = await CreateFileAsync(documentRef, CreationCollisionOption.ReplaceExisting);

                    await JsonDocumentSerializer.SerializeToFileAsync(file, history);
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
                try
                {
                    StorageFile file = await GetFileAsync(documentRef);

                    await file.RenameAsync($"{newName}{JsonDocumentSerializer.FileExtension.Extension}", NameCollisionOption.GenerateUniqueName);

                    documentRef.Updated().Rename(file.DisplayName);
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
                    StorageFile file = await CreateFileAsync(documentRef, CreationCollisionOption.GenerateUniqueName);

                    await JsonDocumentSerializer.SerializeToFileAsync(file, history);

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
    }
}
