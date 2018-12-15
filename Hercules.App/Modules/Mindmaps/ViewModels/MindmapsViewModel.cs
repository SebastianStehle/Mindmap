// ==========================================================================
// MindmapsViewModel.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using GP.Utils;
using GP.Utils.Mvvm;
using Hercules.App.Components;
using Hercules.App.Messages;
using Hercules.Model.ExImport;
using Unity.Attributes;

namespace Hercules.App.Modules.Mindmaps.ViewModels
{
    public sealed class MindmapsViewModel : ViewModelBase
    {
        private readonly IMindmapStore mindmapStore;
        private RelayCommand openCommand;
        private RelayCommand saveCommand;
        private RelayCommand saveAsCommand;
        private RelayCommand createCommand;
        private RelayCommand<IDocumentFileModel> removeCommand;

        public IMindmapStore MindmapStore
        {
            get { return mindmapStore; }
        }

        public ObservableCollection<IDocumentFileModel> RecentFiles
        {
            get { return mindmapStore.AllFiles; }
        }

        [NotifyUI]
        public bool IsLoaded { get; set; }

        [NotifyUI]
        public IDocumentFileModel SelectedFile { get; set; }

        [Dependency]
        public IProcessManager ProcessManager { get; set; }

        [Dependency]
        public ISettingsProvider SettingsProvider { get; set; }

        [Dependency]
        public IDialogService MessageDialogService { get; set; }

        public ICommand RemoveCommand
        {
            get
            {
                return removeCommand ?? (removeCommand = new RelayCommand<IDocumentFileModel>(x =>
                {
                    mindmapStore.RemoveAsync(x).Forget();
                }, x => x != null));
            }
        }

        public ICommand OpenCommand
        {
            get
            {
                return openCommand ?? (openCommand = new RelayCommand(() =>
                {
                    mindmapStore.PickAndAddAsync().Forget();
                }, () => PlattformDetector.IsDesktop));
            }
        }

        public ICommand SaveCommand
        {
            get
            {
                return saveCommand ?? (saveCommand = new RelayCommand(() =>
                {
                    mindmapStore.SaveAsync(mindmapStore.SelectedFile).Forget();
                }, () => SelectedFile != null).DependentOn(this, nameof(SelectedFile)));
            }
        }

        public ICommand SaveAsCommand
        {
            get
            {
                return saveAsCommand ?? (saveAsCommand = new RelayCommand(() =>
                {
                    mindmapStore.SaveAsAsync(mindmapStore.SelectedFile).Forget();
                }, () => PlattformDetector.IsDesktop && SelectedFile != null).DependentOn(this, nameof(SelectedFile)));
            }
        }

        public ICommand CreateCommand
        {
            get
            {
                return createCommand ?? (createCommand = new RelayCommand(async () =>
                {
                    await mindmapStore.AddAsync(LocalizationManager.GetString("MyMindmap"));
                    await mindmapStore.OpenAsync(mindmapStore.AllFiles[0]);
                }));
            }
        }

        public MindmapsViewModel()
        {
        }

        public MindmapsViewModel(IMindmapStore mindmapStore, IMessenger messenger)
        {
            this.mindmapStore = mindmapStore;

            mindmapStore.FileLoaded += MindmapStore_FileLoaded;

            messenger.Register<SaveMessage>(this, OnSave);
            messenger.Register<OpenMessage>(this, OnOpen);
            messenger.Register<ImportMessage>(this, OnImport);
        }

        public void Load()
        {
            LoadAsync().Forget();
        }

        public async Task LoadAsync()
        {
            try
            {
                if (SettingsProvider.IsAlreadyStarted)
                {
                    await mindmapStore.LoadRecentsAsync();
                }
                else
                {
                    await mindmapStore.AddAsync(LocalizationManager.GetString("MyMindmap"));
                }

                await mindmapStore.OpenAsync(mindmapStore.AllFiles.FirstOrDefault());
            }
            finally
            {
                SettingsProvider.IsAlreadyStarted = true;
            }
        }

        private async void OnSave(SaveMessage message)
        {
            await mindmapStore.SaveAsync(mindmapStore.SelectedFile);
            await mindmapStore.StoreRecentsAsync();
            await mindmapStore.StoreBackupAsync();

            message.Callback();
        }

        public void OnOpen(OpenMessage message)
        {
            mindmapStore.AddAsync(message.File).Forget();
        }

        public void OnImport(ImportMessage message)
        {
            ImportAsync(message.Content);
        }

        public void OnSelectedFileChanged()
        {
            mindmapStore.OpenAsync(SelectedFile).Forget();
        }

        private void ImportAsync(ImportModel model)
        {
            ProcessManager.RunMainProcessAsync(mindmapStore, async () =>
            {
                List<ImportResult> results = null;
                try
                {
                    results = await model.Source.ImportAsync(model.Importer);
                }
                catch
                {
                    await MessageDialogService.AlertLocalizedAsync("ImportFailed_Alert");
                }

                if (results?.Count > 0)
                {
                    foreach (var result in results)
                    {
                        await mindmapStore.AddAsync(result.Name, result.Document);
                    }

                    await mindmapStore.OpenAsync(mindmapStore.AllFiles.FirstOrDefault());
                }
            }).Forget();
        }

        private void MindmapStore_FileLoaded(object sender, DocumentFileEventArgs e)
        {
            SelectedFile = e.File;
        }
    }
}
