// ==========================================================================
// DocumentStore.cs
// RavenMind Application
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using Newtonsoft.Json;
using RavenMind.Model.Storing.Utils;
using RavenMind.Model.Utils;
using SE.Metro;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.FileProperties;
using Windows.Storage.Streams;

namespace RavenMind.Model.Storing.Json
{
    public sealed class JsonDocumentStore : IDocumentStore
    {
        private const string DefaultSubfolder = "Mindmaps7";
        private readonly JsonSerializer historySerializer = new JsonSerializer();
        private readonly TaskFactory taskFactory = new TaskFactory(new LimitedThreadsScheduler());
        private readonly string subfolderName;
        private StorageFolder localFolder;
        
        public JsonDocumentStore()
            : this(DefaultSubfolder)
        {
        }

        private JsonDocumentStore(string subfolderName)
        {
            Guard.NotNullOrEmpty(subfolderName, "subfolderName");

            this.subfolderName = subfolderName;
        }

        public Task<IList<DocumentRef>> LoadAllAsync()
        {
            return taskFactory.StartNew<Task<IList<DocumentRef>>>(() => LoadAllInternalAsync()).Unwrap();
        }

        public Task<DocumentRef> StoreAsync(Document document)
        {
            return taskFactory.StartNew<Task<DocumentRef>>(() => StoreInternalAsync(document)).Unwrap();
        }

        public Task<Document> LoadAsync(Guid documentId)
        {
            return taskFactory.StartNew<Task<Document>>(() => LoadInternalAsync(documentId)).Unwrap();
        }

        public Task DeleteAsync(Guid documentId)
        {
            return taskFactory.StartNew<Task>(() => DeleteInternalAsync(documentId)).Unwrap();
        }

        public Task ClearAsync()
        {
            return taskFactory.StartNew<Task>(() => ClearInternalAsync()).Unwrap();
        }

        private async Task<IList<DocumentRef>> LoadAllInternalAsync()
        {
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

                JsonHistory history = new JsonSerializer().Deserialize<JsonHistory>(new JsonTextReader(new StreamReader(normalStream)));

                Document document = new Document(history.Id, history.Name);

                foreach (JsonHistoryStep step in history.Steps.Reverse<JsonHistoryStep>())
                {
                    document.BeginTransaction(step.Name, timestamp: step.Date);

                    foreach (JsonHistoryStepCommand jsonCommand in step.Commands)
                    {
                        Type commandType = Type.GetType(jsonCommand.CommandType);

                        CommandBase command = (CommandBase)Activator.CreateInstance(commandType, jsonCommand.Properties, document);

                        document.Apply(command);
                    }

                    document.CommitTransaction();
                }

                return document;
            }
        }

        private async Task<DocumentRef> StoreInternalAsync(Document document)
        {
            await EnsureFolderAsync();

            JsonHistory history = new JsonHistory { Name = document.Name, Id = document.Id };

            foreach (var transaction in document.UndoRedoManager.History.OfType<CompositeUndoRedoAction>())
            {
                JsonHistoryStep jsonStep = new JsonHistoryStep { Name = transaction.Name, Date = transaction.Date };

                foreach (CommandBase command in transaction.Commands)
                {
                    JsonHistoryStepCommand jsonCommand = new JsonHistoryStepCommand { CommandType = command.GetType().AssemblyQualifiedName };

                    command.Save(jsonCommand.Properties);

                    jsonStep.Commands.Add(jsonCommand);
                }

                history.Steps.Add(jsonStep);
            }

            string json = await JsonConvert.SerializeObjectAsync(history);

            await Task.WhenAll(
                localFolder.WriteTextAsync(document.Id + ".mmn", document.Name),
                localFolder.WriteTextAsync(document.Id + ".mmd", json));

            return new DocumentRef(document.Id, document.Name, DateTime.Now);
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
