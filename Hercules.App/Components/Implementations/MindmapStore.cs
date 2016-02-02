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
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Xaml;
using GP.Utils;
using GP.Utils.Mvvm;
using Hercules.Model;
using Hercules.Model.Storing;
using PropertyChanged;

// ReSharper disable InvertIf
// ReSharper disable ImplicitlyCapturedClosure

namespace Hercules.App.Components.Implementations
{
    [ImplementPropertyChanged]
    public sealed class MindmapStore : IMindmapStore
    {
        private readonly ObservableCollection<IDocumentFileModel> allFiles = new ObservableCollection<IDocumentFileModel>();
        private readonly DocumentFileRecentList recentList = new DocumentFileRecentList();
        private readonly DispatcherTimer autoSaveTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(30) };
        private readonly IDialogService dialogService;
        private readonly IProcessManager processManager;
        private DocumentFileModel selectedFile;

        public event EventHandler<DocumentFileEventArgs> FileLoaded;

        public ObservableCollection<IDocumentFileModel> AllFiles
        {
            get { return allFiles; }
        }

        public IDocumentFileModel SelectedFile
        {
            get { return selectedFile; }
        }

        public MindmapStore(IDialogService dialogService, IProcessManager processManager)
        {
            this.dialogService = dialogService;
            this.processManager = processManager;

            autoSaveTimer.Tick += AutoSaveTimer_Tick;
            autoSaveTimer.Start();
        }

        private void AutoSaveTimer_Tick(object sender, object e)
        {
            if (selectedFile != null)
            {
                selectedFile.SaveSilentAsync().Forget();
            }
        }

        public async Task LoadRecentsAsync()
        {
            IEnumerable<DocumentFile> files = await recentList.LoadAsync();

            files.Foreach(x => allFiles.Add(new DocumentFileModel(x, dialogService)));
        }

        public async Task OpenDocumentRecentAsync()
        {
            if (allFiles.Count > 0)
            {
                await OpenDocumentAsync(allFiles[0]);
            }
        }

        public async Task AddFromFileAsync()
        {
            StorageFile file = await PickFileAsync(DocumentFile.Extension);

            await AddAsync(file);
        }

        public async Task AddAsync(StorageFile file)
        {
            if (file != null)
            {
                DocumentFileModel model = allFiles.OfType<DocumentFileModel>().FirstOrDefault(x => string.Equals(x.Path, file.Path, StringComparison.OrdinalIgnoreCase));

                if (model == null)
                {
                    model = new DocumentFileModel(await DocumentFile.OpenAsync(file), dialogService);

                    allFiles.Insert(0, model);
                }

                await OpenDocumentAsync(model);
            }
        }

        public Task OpenDocumentAsync(IDocumentFileModel file)
        {
            return ForFileAsync(file, m => m != selectedFile, async model =>
            {
                await processManager.RunMainProcessAsync(this, async () =>
                {
                    if (await model.OpenAsync())
                    {
                        if (selectedFile != null && !selectedFile.HasChanges)
                        {
                            selectedFile.Close();
                        }

                        selectedFile = model;
                    }
                    else
                    {
                        allFiles.Remove(file);
                    }

                    OnFileLoaded(selectedFile);
                });
            });
        }

        public async Task AddAsync(string name, Document document = null)
        {
            Guard.ValidFileName(name, nameof(name));

            DocumentFileModel model = new DocumentFileModel(DocumentFile.CreateNew(name, document), dialogService);

            if (await model.File.SaveToLocalFolderAsync())
            {
                allFiles.Insert(0, model);
            }
        }

        public Task SaveRecentsAsync()
        {
            return recentList.SaveAsync(allFiles.OfType<DocumentFileModel>().Select(x => x.File).ToList());
        }

        public async Task SaveAsAsync()
        {
            if (selectedFile != null)
            {
                await selectedFile.SaveAsAsync();
            }
        }

        public async Task SaveAsync(bool hideDialogs = false)
        {
            if (selectedFile != null)
            {
                await selectedFile.SaveAsync(hideDialogs);
            }
        }

        public Task RemoveAsync(IDocumentFileModel file)
        {
            return ForFileAsync(file, m => true, async model =>
            {
                if (await model.RemoveAsync())
                {
                    allFiles.Remove(model);

                    if (selectedFile == model)
                    {
                        selectedFile = null;

                        OnFileLoaded(new DocumentFileEventArgs(null));
                    }
                }
            });
        }

        public Task RenameAsync(IDocumentFileModel file, string newName)
        {
            Guard.ValidFileName(newName, nameof(newName));

            return ForFileAsync(file, m => true, async model =>
            {
                if (!await model.RenameAsync(newName))
                {
                    if (selectedFile != model)
                    {
                        allFiles.Remove(file);
                    }
                }
            });
        }

        private static Task ForFileAsync(IDocumentFileModel file, Predicate<DocumentFileModel> predicate, Func<DocumentFileModel, Task> action)
        {
            DocumentFileModel model = file as DocumentFileModel;

            return model != null && predicate(model) ? action(model) : Task.FromResult(false);
        }

        private void OnFileLoaded(IDocumentFileModel file)
        {
            OnFileLoaded(new DocumentFileEventArgs(file));
        }

        private void OnFileLoaded(DocumentFileEventArgs e)
        {
            FileLoaded?.Invoke(this, e);
        }

        private static Task<StorageFile> PickFileAsync(string extension)
        {
            FileOpenPicker fileOpenPicker = new FileOpenPicker();

            fileOpenPicker.FileTypeFilter.Add(extension);

            return fileOpenPicker.PickSingleFileAsync().AsTask();
        }
    }
}
