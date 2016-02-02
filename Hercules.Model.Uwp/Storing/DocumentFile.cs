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
using GP.Utils;
using Hercules.Model.Storing.Json;

// ReSharper disable InvertIf
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

        public bool IsInLocalFolder
        {
            get { return IsInFolder(ApplicationData.Current.LocalFolder); }
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

        private DocumentFile(Document document, string name)
            : this(name)
        {
            OpenInternal(document);
        }

        public static async Task<DocumentFile> OpenAsync(StorageFile file)
        {
            Guard.NotNull(file, nameof(file));

            return new DocumentFile(file, await file.GetBasicPropertiesAsync());
        }

        public static DocumentFile CreateNew(string name, Document document = null)
        {
            Guard.ValidFileName(name, nameof(name));

            return new DocumentFile(document ?? Document.CreateNew(name), name);
        }

        public static DocumentFile Create(string name, Document document)
        {
            Guard.ValidFileName(name, nameof(name));

            return new DocumentFile(document, name);
        }

        public bool IsInFolder(StorageFolder folder)
        {
            return file != null && folder != null && file.Path.StartsWith(folder.Path, StringComparison.OrdinalIgnoreCase);
        }

        public Task<bool> RenameAsync(string newName)
        {
            Guard.ValidFileName(newName, nameof(newName));

            return DoAsync(() => file != null, async () =>
            {
                await FileQueue.RenameAsync(file, newName, Extension);

                name = newName;
            });
        }

        public Task<bool> OpenAsync()
        {
            if (document != null)
            {
                return Task.FromResult(true);
            }

            return DoAsync(() => document == null && file != null, async () =>
            {
                Document newDocument = await FileQueue.OpenAsync(file);

                OpenInternal(newDocument);
            });
        }

        public Task<bool> SaveToLocalFolderAsync()
        {
            return SaveToAsync(ApplicationData.Current.LocalFolder);
        }

        public async Task<bool> SaveToAsync(StorageFolder folder)
        {
            return folder != null && await SaveAsync(await folder.CreateFileAsync(name + Extension, CreationCollisionOption.GenerateUniqueName));
        }

        public Task<bool> SaveAsAsync(StorageFile newFile)
        {
            return SaveAsync(newFile);
        }

        public Task<bool> SaveAsync()
        {
            return SaveAsync(file);
        }

        private Task<bool> SaveAsync(StorageFile targetFile)
        {
            return DoAsync(() => document != null && targetFile != null, async () =>
            {
                await FileQueue.SaveAsync(document, targetFile);

                try
                {
                    if (file != null && !string.Equals(file.Path, targetFile.Path, StringComparison.OrdinalIgnoreCase))
                    {
                        if (IsInLocalFolder)
                        {
                            await file.DeleteAsync(StorageDeleteOption.PermanentDelete);
                        }
                    }
                }
                finally
                {
                    file = targetFile;

                    if (file != targetFile)
                    {
                        name = file.DisplayName;
                    }
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

                    modifiedUtc = DateTime.UtcNow;

                    hasSucceeded = true;
                    hasChanges = false;
                }
                catch (FileNotFoundException)
                {
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
