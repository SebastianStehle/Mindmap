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
using System.Composition;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.FileProperties;
using Windows.Storage.Streams;

namespace RavenMind.Model.Storing.Json
{
    [Export]
    [Export(typeof(IDocumentStore))]
    public sealed class JsonDocumentStore : IDocumentStore
    {
        private const string DefaultSubfolder = "Mindmaps2";
        private readonly JsonSerializer historySerializer = new JsonSerializer();
        private readonly StorageFolder localFolder;
        private readonly TaskScheduler taskScheduler = new LimitedThreadsScheduler();
        
        public JsonDocumentStore()
            : this(DefaultSubfolder)
        {
        }

        public JsonDocumentStore(string subfolderName)
        {
            Guard.NotNullOrEmpty(subfolderName, "subfolderName");

            localFolder = ApplicationData.Current.LocalFolder.CreateFolder(subfolderName);
        }

        public async Task<IList<DocumentRef>> LoadAllAsync()
        {
            TaskFactory<IList<DocumentRef>> taskFactory = new TaskFactory<IList<DocumentRef>>(taskScheduler);

            try
            {
                return await taskFactory.StartNew(() =>
                {
                    List<DocumentRef> documentReferences = new List<DocumentRef>();

                    IEnumerable<StorageFile> files = localFolder.GetFiles();

                    foreach (StorageFile file in files)
                    {
                        if (file.FileType == ".xml")
                        {
                            BasicProperties properties = file.GetProperties();

                            string name = file.ReadText();

                            if (!string.IsNullOrWhiteSpace(name))
                            {
                                documentReferences.Add(new DocumentRef(Guid.Parse(file.DisplayName), name, properties.DateModified));
                            }
                        }
                    }

                    ////return documentReferences.OrderByDescending(x => x.LastUpdate).ToList();

                    return new List<DocumentRef>();
                });
            }
            catch (AggregateException e)
            {
                throw e.InnerException;
            }
        }

        public async Task<Document> LoadAsync(Guid documentId)
        {
            TaskFactory<Document> taskFactory = new TaskFactory<Document>(taskScheduler);

            try
            {
                return await taskFactory.StartNew(() =>
                {
                    StorageFile file = localFolder.GetFile(documentId + ".mml");

                    using (IRandomAccessStream stream = file.Open())
                    {
                        ////Stream normalStream = stream.AsStreamForRead();

                        ////History history = (History)historySerializer.Deserialize(normalStream);

                        ////Document document = new Document(history.Id, history.Name);

                        ////foreach (HistoryStep step in history.Steps.Reverse<HistoryStep>())
                        ////{
                        ////    document.BeginTransaction(step.Name, timestamp: step.Date);

                        ////    foreach (CommandBase command in step.Commands)
                        ////    {
                        ////        document.Apply(command);
                        ////    }

                        ////    document.CommitTransaction();
                        ////}

                        return null;
                    }
                });
            }
            catch (AggregateException e)
            {
                throw e.InnerException;
            }
        }

        public async Task DeleteAsync(Guid documentId)
        {
            TaskFactory factory = new TaskFactory(taskScheduler);

            try
            {
                await factory.StartNew(() =>
                {
                    localFolder.TryDeleteIfExists(documentId + ".xml");
                    localFolder.TryDeleteIfExists(documentId + ".mml");
                });
            }
            catch (AggregateException e)
            {
                throw e.InnerException;
            }
        }

        public Task<DocumentRef> StoreAsync(Document document)
        {
            if (document == null)
            {
                throw new ArgumentNullException("document");
            }

            if (string.IsNullOrWhiteSpace(document.Name))
            {
                throw new ArgumentException("Document name cannot be null or empty.", "document");
            }

            ////TaskFactory<DocumentRef> taskFactory = new TaskFactory<DocumentRef>(taskScheduler);

            ////History history = new History { Name = document.Name, Id = document.Id };

            ////foreach (var item in document.UndoRedoManager.History.OfType<CompositeUndoRedoAction>())
            ////{
            ////    HistoryStep step = new HistoryStep { Name = item.Name, Date = item.Date };

            ////    foreach (IUndoRedoAction child in item.Actions)
            ////    {
            ////        step.Commands.Add(child.Command);
            ////    }

            ////    history.Steps.Add(step);
            ////}

            ////byte[] buffer = history.Serialize(historySerializer);

            ////try
            ////{
            ////    return await taskFactory.StartNew(() =>
            ////    {
            ////        localFolder.WriteText(document.Id + ".xml", document.Name);
            ////        localFolder.WriteData(document.Id + ".mml", buffer);
                
            ////        return new DocumentRef(document.Id, document.Name, DateTime.Now);
            ////    });
            ////}
            ////catch (AggregateException e)
            ////{
            ////    throw e.InnerException;
            ////}

            return null;
        }

        public async Task ClearAsync()
        {
            TaskFactory factory = new TaskFactory(taskScheduler);

            try
            {
                await factory.StartNew(() =>
                {
                    localFolder.Delete();
                });
            }
            catch (AggregateException e)
            {
                throw e.InnerException;
            }
        }
    }
}
