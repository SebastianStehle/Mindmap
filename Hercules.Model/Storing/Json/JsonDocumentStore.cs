// ==========================================================================
// JsonDocumentStore.cs
// Hercules Mindmap App
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
using Newtonsoft.Json;
using Windows.Storage;
using Windows.Storage.FileProperties;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media.Imaging;

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

        public async Task<DocumentRef> StoreAsync(Document document, Func<IRandomAccessStream, Task> saveScreenshot)
        {
            Guard.NotNull(document, nameof(document));

            InMemoryRandomAccessStream screenshot = await WriteScreenshotToMemoryAsync(document.Id, saveScreenshot);

            return await taskFactory.StartNew(() => StoreInternalAsync(document, screenshot)).Unwrap();
        }

        public Task<Document> LoadAsync(Guid documentId)
        {
            Guard.NotEmpty(documentId, nameof(documentId));

            return taskFactory.StartNew(() => LoadInternalAsync(documentId)).Unwrap();
        }

        public Task DeleteAsync(Guid documentId)
        {
            Guard.NotEmpty(documentId, nameof(documentId));

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
                        documentReferences.Add(new DocumentRef(Guid.Parse(file.DisplayName), name, properties.DateModified, LoadScreenshotAsync));
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

        private async Task<DocumentRef> StoreInternalAsync(Document document, InMemoryRandomAccessStream screenshot)
        {
            Guard.NotNull(document, nameof(document));;

            await EnsureFolderAsync();

            JsonHistory jsonHistory = new JsonHistory(document);

            await Task.WhenAll(
                WriteScreenshotAsync(jsonHistory, screenshot),
                WriteNameAsync(jsonHistory),
                WriteContentAsync(jsonHistory));
            
            return new DocumentRef(document.Id, document.Title, DateTime.Now, LoadScreenshotAsync);
        }

        private async Task<InMemoryRandomAccessStream> WriteScreenshotToMemoryAsync(Guid documentId, Func<IRandomAccessStream, Task> saveScreenshot)
        {
            InMemoryRandomAccessStream stream = null;

            if (saveScreenshot != null)
            {
                stream = new InMemoryRandomAccessStream();

                await saveScreenshot(stream);

                stream.Seek(0);
            }

            return stream;
        }

        private Task WriteScreenshotAsync(JsonHistory history, InMemoryRandomAccessStream screenshotStream)
        {
            string fileName = $"{history.Id}.png";

            return localFolder.TryWriteDataAsync(fileName, screenshotStream);
        }

        private Task WriteNameAsync(JsonHistory history)
        {
            string fileName = $"{history.Id}.mmn";

            return localFolder.WriteTextAsync(fileName, history.Name);
        }

        private Task WriteContentAsync(JsonHistory history)
        {
            string fileName = $"{history.Id}.mmd";

            return history.SerializeAsJsonAsync(localFolder, fileName, historySerializerSettings);
        }

        private async Task<bool> LoadScreenshotAsync(Guid documentId, BitmapImage image)
        {
            try
            {
                string fileName = $"{documentId}.png";

                StorageFile file = await localFolder.GetFileAsync(fileName);

                using (IRandomAccessStream stream = await file.OpenAsync(FileAccessMode.ReadWrite))
                {
                    await image.SetSourceAsync(stream);
                }

                return true;
            }
            catch (FileNotFoundException)
            {
                return false;
            }
        }

        private async Task DeleteInternalAsync(Guid documentId)
        {
            await EnsureFolderAsync();

            await Task.WhenAll(
                localFolder.TryDeleteIfExistsAsync(documentId + ".png"),
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
