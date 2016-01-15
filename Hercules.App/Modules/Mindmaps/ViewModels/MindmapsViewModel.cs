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
using Hercules.Model.Storing;
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

        public ObservableCollection<DocumentFile> RecentFiles
        {
            get
            {
                return mindmapStore.AllDocuments;
            }
        }

        [NotifyUI]
        public bool IsLoaded { get; set; }

        [NotifyUI]
        public DocumentFile SelectedFile { get; set; }

        [Dependency]
        public IProcessManager ProcessManager { get; set; }

        [Dependency]
        public IMessageDialogService MessageDialogService { get; set; }

        public RelayCommand OpenCommand
        {
            get
            {
                return openCommand ?? (openCommand = new RelayCommand(() =>
                {
                    mindmapStore.OpenAsync().Forget();
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
                }));
            }
        }

        public RelayCommand SaveAsCommand
        {
            get
            {
                return saveAsCommand ?? (saveAsCommand = new RelayCommand(() =>
                {
                    mindmapStore.SaveToFileAsync().Forget();
                }));
            }
        }

        public RelayCommand CreateCommand
        {
            get
            {
                return createCommand ?? (createCommand = new RelayCommand(() =>
                {
                    mindmapStore.CreateAsync().Forget();
                }));
            }
        }

        public MindmapsViewModel()
        {
        }

        public MindmapsViewModel(IMindmapStore mindmapStore, IMessenger messenger)
        {
            this.mindmapStore = mindmapStore;

            mindmapStore.DocumentLoaded += MindmapStore_DocumentLoaded;

            messenger.Register<ImportMessage>(this, OnImport);

            messenger.Register<OpenMindmapMessage>(this, OnOpen);
        }

        public void OnOpen(OpenMindmapMessage message)
        {
        }

        public void OnImport(ImportMessage message)
        {
            ImportAsync(message.Content);
        }

        private void ImportAsync(ImportModel model)
        {
            ProcessManager.RunMainProcessAsync(mindmapStore, async () =>
            {
                try
                {
                    List<ImportResult> results = await model.Source.ImportAsync(model.Importer);

                    if (results.Count > 0)
                    {
                        foreach (var result in results)
                        {
                            //await mindmapStore.AddAsync(result.Name, result.Document);
                            //await mindmapStore.LoadAsync(mindmapStore.AllDocuments[0]);

                            //SelectedMindmap = mindmapStore.AllDocuments[0];
                        }
                    }
                }
                catch
                {
                    string content = LocalizationManager.GetString("ImportFailed_Content");
                    string heading = LocalizationManager.GetString("ImportFailed_Heading");

                    await MessageDialogService.AlertAsync(content, heading);
                }
            }).Forget();
        }

        private void MindmapStore_DocumentLoaded(object sender, DocumentFileEventArgs e)
        {
            SelectedFile = e.File;
        }

        public async Task LoadAsync()
        {
            await mindmapStore.LoadRecentAsync();

            SelectedFile = mindmapStore.LoadedDocument;
        }
    }
}
