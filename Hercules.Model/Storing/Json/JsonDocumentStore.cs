// ==========================================================================
// DocumentStore.cs
// Hercules Application
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using Hercules.Model.Storing.Utils;
using Hercules.Model.Utils;
using GP.Windows;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.FileProperties;
using Windows.Storage.Streams;
using Newtonsoft.Json;

namespace Hercules.Model.Storing.Json
{
    public sealed class JsonDocumentStore : IDocumentStore
    {
        private const string DefaultSubfolder = "Hercules66";
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

        public Task<DocumentRef> StoreAsync(Document document)
        {
            return taskFactory.StartNew(() => StoreInternalAsync(document)).Unwrap();
        }

        public Task<Document> LoadAsync(Guid documentId)
        {
            return taskFactory.StartNew(() => LoadInternalAsync(documentId)).Unwrap();
        }

        public Task DeleteAsync(Guid documentId)
        {
            return taskFactory.StartNew(() => DeleteInternalAsync(documentId)).Unwrap();
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

            foreach (StorageFile file in files)
            {
                if (file.FileType.Equals(".mmn", StringComparison.OrdinalIgnoreCase))
                {
                    BasicProperties properties = await file.GetBasicPropertiesAsync();

                    string name = await FileIO.ReadTextAsync(file);

                    if (!string.IsNullOrWhiteSpace(name))
                    {
                        documentReferences.Add(new DocumentRef(Guid.Parse(file.DisplayName), name, properties.DateModified));
                    }
                }
            }

            return documentReferences.OrderByDescending(x => x.LastUpdate).ToList();
        }

        private async Task<Document> LoadInternalAsync(Guid documentId)
        {
            await EnsureFolderAsync();

            StorageFile file = await localFolder.GetFileAsync(documentId + ".mmd");

            using (IRandomAccessStream stream = await file.OpenReadAsync())
            {
                Stream normalStream = stream.AsStreamForRead();

                JsonHistory history = normalStream.DeserializeAsJsonFromStream<JsonHistory>(historySerializerSettings);

                Document document = history.ToDocument();

                return document;
            }
        }

        private async Task<DocumentRef> StoreInternalAsync(Document document)
        {
            await EnsureFolderAsync();

            JsonHistory jsonHistory = new JsonHistory(document);

            await Task.WhenAll(
                WriteNameAsync(jsonHistory),
                WriteContentAsync(jsonHistory));
            
            return new DocumentRef(document.Id, document.Name, DateTime.Now);
        }

        private Task WriteNameAsync(JsonHistory history)
        {
            string fileName = $"{history.Id}.mmn";

            return localFolder.WriteTextAsync(fileName, history.Name);
        }

        private async Task WriteContentAsync(JsonHistory history)
        {
            string fileName = $"{history.Id}.mmd";

            StorageFile file = await localFolder.CreateFileAsync(fileName, CreationCollisionOption.ReplaceExisting);

            using (Stream fileStream = await file.OpenStreamForWriteAsync())
            {
                history.SerializeAsJsonToStream(fileStream, historySerializerSettings);
            }
        } 

        private async Task DeleteInternalAsync(Guid documentId)
        {
            await EnsureFolderAsync();

            await Task.WhenAll(
                localFolder.TryDeleteIfExistsAsync(documentId + ".mmn"),
                localFolder.TryDeleteIfExistsAsync(documentId + ".mmd"));
        }

        private async Task ClearInternalAsync()
        {
            await EnsureFolderAsync();

            await localFolder.DeleteAsync().AsTask();
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
