// ==========================================================================
// MindmapsViewModel.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using GP.Utils;
using GP.Utils.Mvvm;
using Hercules.App.Components;
using Hercules.App.Messages;
using Hercules.Model.ExImport;
using Microsoft.Practices.Unity;
using PropertyChanged;

namespace Hercules.App.Modules.Mindmaps.ViewModels
{
    [ImplementPropertyChanged]
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

        public RelayCommand<IDocumentFileModel> RemoveCommand
        {
            get
            {
                return removeCommand ?? (removeCommand = new RelayCommand<IDocumentFileModel>(x =>
                {
                    mindmapStore.RemoveAsync(x).Forget();
                }, x => x != null));
            }
        }

        public RelayCommand OpenCommand
        {
            get
            {
                return openCommand ?? (openCommand = new RelayCommand(() =>
                {
                    mindmapStore.AddFromFileAsync().Forget();
                }));
            }
        }

        public RelayCommand SaveCommand
        {
            get
            {
                return saveCommand ?? (saveCommand = new RelayCommand(() =>
                {
                    mindmapStore.SaveAsync().Forget();
                }, () => SelectedFile != null).DependentOn(this, nameof(SelectedFile)));
            }
        }

        public RelayCommand SaveAsCommand
        {
            get
            {
                return saveAsCommand ?? (saveAsCommand = new RelayCommand(() =>
                {
                    mindmapStore.SaveAsAsync().Forget();
                }, () => SelectedFile != null).DependentOn(this, nameof(SelectedFile)));
            }
        }

        public RelayCommand CreateCommand
        {
            get
            {
                return createCommand ?? (createCommand = new RelayCommand(() =>
                {
                    mindmapStore.AddAsync(LocalizationManager.GetString("MyMindmap")).Forget();
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

                await mindmapStore.OpenDocumentRecentAsync();
            }
            finally
            {
                SettingsProvider.IsAlreadyStarted = true;
            }
        }

        private async void OnSave(SaveMessage message)
        {
            await mindmapStore.SaveAsync();
            await mindmapStore.SaveRecentsAsync();

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
            mindmapStore.OpenDocumentAsync(SelectedFile).Forget();
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

                    await mindmapStore.OpenDocumentRecentAsync();
                }
            }).Forget();
        }

        private void MindmapStore_FileLoaded(object sender, DocumentFileEventArgs e)
        {
            SelectedFile = e.File;
        }
    }
}
