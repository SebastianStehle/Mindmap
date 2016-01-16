// ==========================================================================
// DocumentFile.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System;
using System.IO;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.FileProperties;
using Windows.Storage.Streams;
using GP.Utils;
using Hercules.Model.Storing.Json;

// ReSharper disable PossibleNullReferenceException
// ReSharper disable SuggestBaseTypeForParameter

namespace Hercules.Model.Storing
{
    public class DocumentFile
    {
        private StorageFile file;
        private DateTimeOffset modifiedUtc = DateTimeOffset.UtcNow;
        private Document document;
        private bool hasChanges;
        private string name;

        public event EventHandler Changed;

        public string Name
        {
            get { return name; }
        }

        public string Path
        {
            get { return file?.Path; }
        }

        public bool HasChanges
        {
            get { return hasChanges; }
        }

        public StorageFile File
        {
            get { return file; }
        }

        public Document Document
        {
            get { return document; }
        }

        public DateTimeOffset ModifiedUtc
        {
            get { return modifiedUtc; }
        }

        public static string Extension
        {
            get { return JsonDocumentSerializer.FileExtension.Extension; }
        }

        private DocumentFile(string name)
        {
            this.name = name;
        }

        private DocumentFile(StorageFile file, BasicProperties properties)
            : this(file.DisplayName)
        {
            this.file = file;

            modifiedUtc = properties.DateModified.UtcDateTime;
        }

        private DocumentFile(Document document, string name, bool initialize)
            : this(name)
        {
            OpenInternal(document);

            hasChanges = true;

            if (initialize)
            {
                document.Root.ChangeTextTransactional(name);
            }
        }

        public static DocumentFile Create(string name, Document document)
        {
            Guard.ValidFileName(name, nameof(name));

            return new DocumentFile(document, name, false);
        }

        public static DocumentFile CreateNew(string name)
        {
            Guard.ValidFileName(name, nameof(name));

            return new DocumentFile(new Document(Guid.NewGuid()), name, true);
        }

        public static async Task<DocumentFile> OpenAsync(StorageFile file)
        {
            Guard.NotNull(file, nameof(file));

            return new DocumentFile(file, await file.GetBasicPropertiesAsync());
        }

        public async Task<bool> RenameAsync(string newName)
        {
            Guard.ValidFileName(newName, nameof(newName));

            bool hasRenamed = false;

            if (file != null)
            {
                try
                {
                    await FileQueue.EnqueueAsync(() => file.RenameAsync(newName + JsonDocumentSerializer.FileExtension, NameCollisionOption.GenerateUniqueName).AsTask());

                    name = newName;

                    hasRenamed = true;
                }
                catch (FileNotFoundException)
                {
                    hasChanges = true;

                    file = null;
                    throw;
                }
            }

            return hasRenamed;
        }

        public async Task<bool> OpenAsync()
        {
            bool hasOpened = false;

            if (document == null)
            {
                Document newDocument = null;

                await FileQueue.EnqueueAsync(async () =>
                {
                    using (IRandomAccessStream stream = await file.OpenReadAsync())
                    {
                        if (stream.Size > 0)
                        {
                            newDocument = JsonDocumentSerializer.Deserialize(stream.AsStreamForRead());
                        }
                    }
                });

                hasChanges = newDocument == null;

                if (newDocument == null)
                {
                    newDocument = new Document(Guid.NewGuid());

                    newDocument.Root.ChangeTextTransactional(file.DisplayName);
                }

                OpenInternal(newDocument);

                hasOpened = true;
            }

            return hasOpened;
        }

        public Task<bool> SaveAsAsync(StorageFile newFile)
        {
            return SaveAsync(newFile);
        }

        public Task<bool> SaveAsync()
        {
            return SaveAsync(file);
        }

        private async Task<bool> SaveAsync(StorageFile targetFile)
        {
            bool hasSucceeded = false;

            if (document != null && targetFile != null)
            {
                try
                {
                    JsonHistory history = new JsonHistory(document);

                    await FileQueue.EnqueueAsync(async () =>
                    {
                        using (StorageStreamTransaction transaction = await targetFile.OpenTransactedWriteAsync())
                        {
                            JsonDocumentSerializer.Serialize(history, transaction.Stream.AsStreamForWrite());

                            await transaction.CommitAsync();
                        }
                    });

                    modifiedUtc = DateTime.UtcNow;

                    hasSucceeded = true;
                    hasChanges = false;

                    file = targetFile;
                }
                catch (FileNotFoundException)
                {
                    hasChanges = true;

                    file = null;

                    throw;
                }
            }

            return hasSucceeded;
        }

        public void Close()
        {
            CloseInternal();

            hasChanges = false;
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

            Changed?.Invoke(this, e);
        }
    }
}
