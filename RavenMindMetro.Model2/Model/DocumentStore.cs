// ==========================================================================
// DocumentStore.cs
// RavenMind Application
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using RavenMind.Model.Utils;
using System;
using System.Collections.Generic;
using System.Composition;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Windows.Storage;
using Windows.Storage.FileProperties;
using Windows.Storage.Streams;

namespace RavenMind.Model
{
    /// <summary>
    /// Default implementation of the <see cref="IDocumentStore"/> interface to load and store mindmaps in the file system.
    /// </summary>
    [Export]
    [Export(typeof(IDocumentStore))]
    public sealed class DocumentStore : IDocumentStore
    {
        #region Fields

        private readonly XmlSerializer historySerializer = new XmlSerializer(typeof(History));
        private readonly StorageFolder localFolder;
        private readonly TaskScheduler taskScheduler = new LimitedThreadsScheduler();

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="DocumentStore"/> class.
        /// </summary>
        public DocumentStore()
            : this("Mindmaps2")
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DocumentStore"/> class with the name of the subfolder.
        /// </summary>
        /// <param name="subfolderName">The name of the subfolder. Cannot be null or empty.</param>
        /// <exception cref="ArgumentException"><paramref name="subfolderName"/> is null or empty.</exception>
        public DocumentStore(string subfolderName)
        {
            if (string.IsNullOrWhiteSpace(subfolderName))
            {
                throw new ArgumentException("Cannot be null or empty.", subfolderName);
            }

            localFolder = ApplicationData.Current.LocalFolder.CreateFolder(subfolderName);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Loads all documents.
        /// </summary>q
        /// <returns>
        /// The list of documents from the store.
        /// </returns>
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

                    return documentReferences.OrderByDescending(x => x.LastUpdate).ToList();
                });
            }
            catch (AggregateException e)
            {
                throw e.InnerException;
            }
        }

        /// <summary>
        /// Loads the document for the specified document info reference.
        /// </summary>
        /// <param name="documentId">The reference to the document. Cannot be null.</param>
        /// <returns>
        /// The loaded document.
        /// </returns>
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
                        Stream normalStream = stream.AsStreamForRead();

                        History history = (History)historySerializer.Deserialize(normalStream);

                        Document document = new Document(history.Id, history.Name);

                        foreach (HistoryStep step in history.Steps.Reverse<HistoryStep>())
                        {
                            document.BeginTransaction(step.Name, timestamp: step.Date);

                            foreach (DocumentCommandBase command in step.Commands)
                            {
                                document.Apply(command);
                            }

                            document.CommitTransaction();
                        }

                        return document;
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

        public async Task<DocumentRef> StoreAsync(Document document)
        {
            if (document == null)
            {
                throw new ArgumentNullException("document");
            }

            if (string.IsNullOrWhiteSpace(document.Name))
            {
                throw new ArgumentException("Document name cannot be null or empty.", "document");
            }

            TaskFactory<DocumentRef> taskFactory = new TaskFactory<DocumentRef>(taskScheduler);

            History history = new History { Name = document.Name, Id = document.Id };

            foreach (var item in document.UndoRedoManager.History.OfType<CompositeUndoRedoAction>())
            {
                HistoryStep step = new HistoryStep { Name = item.Name, Date = item.Date };

                foreach (UndoRedoAction child in item.Actions)
                {
                    step.Commands.Add(child.Command);
                }

                history.Steps.Add(step);
            }

            byte[] buffer = history.Serialize(historySerializer);

            try
            {
                return await taskFactory.StartNew(() =>
                {
                    localFolder.WriteText(document.Id + ".xml", document.Name);
                    localFolder.WriteData(document.Id + ".mml", buffer);
                
                    return new DocumentRef(document.Id, document.Name, DateTime.Now);
                });
            }
            catch (AggregateException e)
            {
                throw e.InnerException;
            }
        }

        /// <summary>
        /// Removes all documents from the current store.
        /// </summary>
        /// <returns>
        /// The task object that can be used to wait for the async operation.
        /// </returns>
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

        #endregion
    }
}
