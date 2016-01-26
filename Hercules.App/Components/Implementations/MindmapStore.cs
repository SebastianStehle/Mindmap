// ==========================================================================
// MindmapStore.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System;
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
        private readonly ISettingsProvider settingsProvider;
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

        public MindmapStore(IDialogService dialogService, IProcessManager processManager, ISettingsProvider settingsProvider)
        {
            this.dialogService = dialogService;
            this.processManager = processManager;
            this.settingsProvider = settingsProvider;

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
            if (!settingsProvider.IsAlreadyStarted)
            {
                await CreateAsync();

                settingsProvider.IsAlreadyStarted = true;
            }

            if (!settingsProvider.HasFilesCopied)
            {
                await LocalFiles.CopyToDocumentsAsync(recentList);

                settingsProvider.HasFilesCopied = true;
            }

            await recentList.LoadAsync();

            recentList.Files.Foreach(x => allFiles.Add(new DocumentFileModel(x, dialogService)));

            await OpenRecentAsync();
        }

        public async Task OpenRecentAsync()
        {
            if (allFiles.Count > 0)
            {
                await OpenAsync(allFiles[0]);
            }
        }

        public async Task OpenFromFileAsync()
        {
            StorageFile file = await PickFileAsync(DocumentFile.Extension);

            await OpenAsync(file);
        }

        public async Task OpenAsync(StorageFile file)
        {
            if (file != null)
            {
                DocumentFileModel model = allFiles.OfType<DocumentFileModel>().FirstOrDefault(x => string.Equals(x.Path, file.Path, StringComparison.OrdinalIgnoreCase));

                if (model == null)
                {
                    model = new DocumentFileModel(await DocumentFile.OpenAsync(file), dialogService);

                    Add(model);
                }

                await OpenAsync(model);
            }
        }

        public Task OpenAsync(IDocumentFileModel file)
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

                        recentList.Add(model.File);
                    }
                    else
                    {
                        allFiles.Remove(file);
                    }

                    OnFileLoaded(selectedFile);
                });
            });
        }

        public void Add(string name, Document document)
        {
            Add(DocumentFile.Create(name, document));
        }

        private void Add(DocumentFile file)
        {
            Add(new DocumentFileModel(file, dialogService));
        }

        private void Add(DocumentFileModel file)
        {
            allFiles.Insert(0, file);

            if (file.File != null)
            {
                recentList.Add(file.File);
            }
        }

        public async Task CreateAsync()
        {
            DocumentFile file = DocumentFile.CreateNew(LocalizationManager.GetString("MyMindmap"));

            if (await file.SaveToAsync(KnownFolders.DocumentsLibrary))
            {
                Add(file);

                await OpenRecentAsync();
            }
        }

        public async Task SaveAsAsync()
        {
            if (selectedFile != null)
            {
                if (await selectedFile.SaveAsAsync())
                {
                    recentList.Add(selectedFile.File);
                }
            }
        }

        public async Task SaveAsync(bool hideDialogs = false)
        {
            if (selectedFile != null)
            {
                if (await selectedFile.SaveAsync(hideDialogs))
                {
                    recentList.Add(selectedFile.File);
                }
            }
        }

        public Task RemoveAsync(IDocumentFileModel file)
        {
            return ForFileAsync(file, m => true, async model =>
            {
                if (await model.RemoveAsync())
                {
                    allFiles.Remove(model);

                    if (model.File != null)
                    {
                        recentList.Remove(model.File);
                    }

                    if (selectedFile == model)
                    {
                        selectedFile = null;

                        OnFileLoaded((IDocumentFileModel)null);
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
