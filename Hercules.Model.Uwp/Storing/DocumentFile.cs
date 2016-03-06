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
using GP.Utils;

// ReSharper disable InvertIf
// ReSharper disable PossibleNullReferenceException
// ReSharper disable SuggestBaseTypeForParameter

namespace Hercules.Model.Storing
{
    public class DocumentFile
    {
        private StorageFile fileReference;
        private DateTimeOffset fileModified;
        private Document document;
        private string name;
        private bool hasChanges;
        private bool isInLocalFolder;

        public event EventHandler Changed;

        public string Name
        {
            get { return name; }
        }

        public string Path
        {
            get { return fileReference?.Path; }
        }

        public bool IsInLocalFolder
        {
            get { return isInLocalFolder; }
        }

        public bool HasChanges
        {
            get { return hasChanges; }
        }

        public StorageFile File
        {
            get { return fileReference; }
        }

        public Document Document
        {
            get { return document; }
        }

        public DateTimeOffset ModifiedUtc
        {
            get { return fileModified; }
        }

        public static string Extension
        {
            get { return Constants.FileExtension.Extension; }
        }

        private DocumentFile(StorageFile fileReference, Document document = null, bool isInLocalFolder = true, DateTimeOffset? fileModified = null)
        {
            name = fileReference?.DisplayName;

            this.fileModified = fileModified ?? DateTimeOffset.UtcNow;
            this.fileReference = fileReference;
            this.isInLocalFolder = isInLocalFolder;

            OpenInternal(document);
        }

        public static async Task<DocumentFile> OpenAsync(StorageFile file, bool isInLocalFolder)
        {
            Guard.NotNull(file, nameof(file));

            return new DocumentFile(file, null, isInLocalFolder, (await file.GetBasicPropertiesAsync()).DateModified.UtcDateTime);
        }

        public static async Task<DocumentFile> CreateNewAsync(string name, Document document = null)
        {
            StorageFile storageFile = await LocalStore.CreateFileQueuedAsync(name + Extension);

            DocumentFile file = new DocumentFile(storageFile, document ?? Document.CreateNew(name));

            await file.SaveAsync();

            return file;
        }

        public Task<bool> RenameAsync(string newName)
        {
            return DoAsync(() => fileReference != null, async () =>
            {
                await fileReference.RenameQueuedAsync(newName, Extension);
            });
        }

        public Task DeleteAsync()
        {
            return DoAsync(() => fileReference != null && isInLocalFolder, async () =>
            {
                await fileReference.DeleteQueuedAsync();
            });
        }

        public Task<bool> OpenAsync()
        {
            return DoAsync(() => fileReference != null || document != null, async () =>
            {
                if (document == null)
                {
                    Document newDocument = await fileReference.OpenDocumentQueuedAsync();

                    OpenInternal(newDocument);
                }
            });
        }

        public Task<bool> SaveAsAsync(StorageFile newFile)
        {
            return SaveAsync(newFile);
        }

        public Task<bool> SaveAsync()
        {
            return SaveAsync(fileReference);
        }

        private Task<bool> SaveAsync(StorageFile file)
        {
            return DoAsync(() => document != null && file != null, async () =>
            {
                await file.SaveDocumentQueuedAsync(document);

                try
                {
                    if (fileReference != null && !string.Equals(fileReference.Path, file.Path, StringComparison.OrdinalIgnoreCase))
                    {
                        if (IsInLocalFolder)
                        {
                            await fileReference.DeleteQueuedAsync();
                        }

                        isInLocalFolder = false;
                    }
                }
                finally
                {
                    fileReference = file;
                }
            });
        }

        private async Task<bool> DoAsync(Func<bool> predicate, Func<Task> action)
        {
            bool hasSucceeded = false;

            if (predicate())
            {
                try
                {
                    await action();

                    if (fileReference != null)
                    {
                        name = fileReference.DisplayName;
                    }

                    fileModified = DateTime.UtcNow;

                    hasSucceeded = true;
                    hasChanges = false;
                }
                catch (FileNotFoundException)
                {
                    fileReference = null;

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

        private void Document_StateChanged(object sender, StateChangedEventArgs e)
        {
            hasChanges = true;

            Changed?.Invoke(this, e);
        }
    }
}
