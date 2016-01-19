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

// ReSharper disable ImplicitlyCapturedClosure

namespace Hercules.App.Components.Implementations
{
    [ImplementPropertyChanged]
    public sealed class MindmapStore : IMindmapStore
    {
        private readonly DocumentFileRecentList recentList = new DocumentFileRecentList();
        private readonly ObservableCollection<DocumentFileModel> allFiles = new ObservableCollection<DocumentFileModel>();
        private readonly DispatcherTimer autoSaveTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(30) };
        private readonly IDialogService dialogService;
        private readonly IProcessManager processManager;
        private readonly ISettingsProvider settingsProvider;
        private DocumentFileModel selectedFile;

        public event EventHandler<DocumentFileEventArgs> FileLoaded;

        public ObservableCollection<DocumentFileModel> AllFiles
        {
            get { return allFiles; }
        }

        public DocumentFileModel SelectedFile
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
                selectedFile.SaveAsync(true).Forget();
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

            if (allFiles.Count > 0 && selectedFile == null)
            {
                await OpenAsync(allFiles[0]);
            }
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
            StorageFile file = await OpenFileAsync(DocumentFile.Extension);

            await OpenAsync(file);
        }

        public async Task OpenAsync(StorageFile file)
        {
            if (file != null)
            {
                DocumentFileModel fileModel = allFiles.FirstOrDefault(x => string.Equals(x.Path, file.Path, StringComparison.OrdinalIgnoreCase));

                if (fileModel == null)
                {
                    fileModel = new DocumentFileModel(await DocumentFile.OpenAsync(file), dialogService);

                    Add(fileModel);
                }

                await OpenAsync(fileModel);
            }
        }

        public async Task OpenAsync(DocumentFileModel file)
        {
            if (file != null && file != selectedFile)
            {
                await processManager.RunMainProcessAsync(this, async () =>
                {
                    if (await file.OpenAsync())
                    {
                        if (selectedFile != null && !selectedFile.HasChanges)
                        {
                            selectedFile.Close();
                        }

                        selectedFile = file;

                        recentList.Add(file.File);

                        OnFileLoaded(new DocumentFileEventArgs(file));
                    }
                });
            }
        }

        public void Add(string name, Document document)
        {
            Add(DocumentFile.Create(name, document));
        }

        public void Add(DocumentFile file)
        {
            Add(new DocumentFileModel(file, dialogService));
        }

        public void Add(DocumentFileModel file)
        {
            allFiles.Insert(0, file);
        }

        public async Task CreateAsync()
        {
            DocumentFile file = DocumentFile.CreateNew(LocalizationManager.GetString("MyMindmap"));

            await file.SaveToAsync(KnownFolders.DocumentsLibrary);

            Add(file);

            await OpenRecentAsync();
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

        public async Task RemoveAsync(DocumentFileModel file)
        {
            if (file != null)
            {
                if (await file.RemoveAsync())
                {
                    allFiles.Remove(file);

                    recentList.Remove(file.File);

                    if (selectedFile == file)
                    {
                        selectedFile = null;

                        OnFileLoaded(new DocumentFileEventArgs(null));
                    }
                }
            }
        }

        private void OnFileLoaded(DocumentFileEventArgs e)
        {
            FileLoaded?.Invoke(this, e);
        }

        private static Task<StorageFile> OpenFileAsync(string extension)
        {
            FileOpenPicker fileOpenPicker = new FileOpenPicker();

            fileOpenPicker.FileTypeFilter.Add(extension);

            return fileOpenPicker.PickSingleFileAsync().AsTask();
        }
    }
}
