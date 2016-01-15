// ==========================================================================
// MindmapStore.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Search;
using GP.Utils;
using GP.Utils.Mvvm;
using Hercules.Model.Storing;
using Hercules.Model.Storing.Json;
using PropertyChanged;

// ReSharper disable ImplicitlyCapturedClosure

namespace Hercules.App.Components.Implementations
{
    [ImplementPropertyChanged]
    public sealed class MindmapStore : IMindmapStore
    {
        private readonly ObservableCollection<DocumentFile> allDocuments = new ObservableCollection<DocumentFile>();
        private readonly IProcessManager processManager;
        private DocumentFile loadedDocument;

        public event EventHandler<DocumentFileEventArgs> DocumentLoaded;

        public ObservableCollection<DocumentFile> AllDocuments
        {
            get { return allDocuments; }
        }

        public DocumentFile LoadedDocument
        {
            get { return loadedDocument; }
        }

        public MindmapStore(IProcessManager processManager)
        {
            this.processManager = processManager;
        }

        public async Task LoadRecentAsync()
        {
            List<DocumentFile> documentFiles = await DocumentFile.QueryRecentFilesAsync();

            allDocuments.Clear();

            documentFiles.Foreach(x => allDocuments.Add(x));

            if (documentFiles.Count > 0)
            {
                await OpenAsync(documentFiles[0]);
            }
        }

        public Task OpenAsync(DocumentFile file)
        {
            Guard.NotNull(file, nameof(file));

            return processManager.RunMainProcessAsync(this, async () =>
            {
                if (loadedDocument != null)
                {
                    await loadedDocument.SaveAsync();

                    loadedDocument.Close();
                }

                await file.OpenAsync();

                loadedDocument = file;

                OnDocumentLoaded(file);
            });
        }

        public Task SaveAsync()
        {
            return processManager.RunMainProcessAsync(this, async () =>
            {
                if (loadedDocument != null)
                {
                    await loadedDocument.SaveAsync();
                }
            });
        }

        public async Task OpenAsync()
        {
            StorageFile file = await PickOpenAsync(JsonDocumentSerializer.FileExtension.Extension);

            if (file != null)
            {
                DocumentFile document = await DocumentFile.OpenAsync(file);

                document.AddToRecentList();

                await OpenAsync(document);
            }
        }

        public async Task CreateAsync()
        {
            StorageFile file = await PickSaveAsync(JsonDocumentSerializer.FileExtension.Extension);

            if (file != null)
            {
                DocumentFile document = DocumentFile.CreateNew(file);

                document.AddToRecentList();

                await document.SaveAsync();

                await OpenAsync(document);
            }
        }

        public async Task SaveToFileAsync()
        {
            StorageFile file = await PickSaveAsync(JsonDocumentSerializer.FileExtension.Extension);

            if (file != null)
            {
                await processManager.RunMainProcessAsync(this, async () =>
                {
                    if (loadedDocument != null)
                    {
                        await loadedDocument.SaveAsAsync(file);
                    }
                });
            }
        }

        private static async Task<StorageFile> PickSaveAsync(string extension)
        {
            FileSavePicker fileSavePicker = new FileSavePicker { DefaultFileExtension = JsonDocumentSerializer.FileExtension.Extension };

            fileSavePicker.FileTypeChoices.Add(extension, new List<string> { extension });

            StorageFile file = await fileSavePicker.PickSaveFileAsync();

            return file;
        }

        private static async Task<StorageFile> PickOpenAsync(string extension)
        {
            FileOpenPicker fileOpenPicker = new FileOpenPicker();

            fileOpenPicker.FileTypeFilter.Add(extension);

            StorageFile file = await fileOpenPicker.PickSingleFileAsync();

            return file;
        }

        private void OnDocumentLoaded(DocumentFile file)
        {
            DocumentLoaded?.Invoke(this, new DocumentFileEventArgs(file));
        }
    }
}
