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
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GP.Windows;
using GP.Windows.Mvvm;
using Hercules.App.Components;
using Hercules.App.Messages;
using Hercules.Model.ExImport;
using Hercules.Model.Utils;
using Microsoft.Practices.Unity;
using PropertyChanged;

namespace Hercules.App.Modules.Mindmaps.ViewModels
{
    [ImplementPropertyChanged]
    public sealed class MindmapsViewModel : ViewModelBase
    {
        private readonly IMindmapStore mindmapStore;
        private RelayCommand<MindmapRef> deleteCommand;

        public ObservableCollection<MindmapRef> Mindmaps
        {
            get
            {
                return mindmapStore.AllMindmaps;
            }
        }

        [NotifyUI]
        public bool IsLoaded { get; set; }

        [NotifyUI]
        public MindmapRef SelectedMindmap { get; set; }

        [Dependency]
        public IProcessManager ProcessManager { get; set; }

        [Dependency]
        public IMessageDialogService MessageDialogService { get; set; }

        public RelayCommand<MindmapRef> DeleteCommand
        {
            get
            {
                return deleteCommand ?? (deleteCommand = new RelayCommand<MindmapRef>(async item =>
                {
                    string content = ResourceManager.GetString("DeleteMindmap_Content");
                    string message = ResourceManager.GetString("DeleteMindmap_Heading");

                    if (await MessageDialogService.ConfirmAsync(content, message))
                    {
                        await mindmapStore.DeleteAsync(item);
                    }
                }));
            }
        }

        public MindmapsViewModel()
        {
            MessengerInstance.Register<ImportMessage>(this, OnImport);
        }

        public MindmapsViewModel(IMindmapStore mindmapStore)
            : this()
        {
            this.mindmapStore = mindmapStore;

            mindmapStore.DocumentLoaded += MindmapStore_DocumentLoaded;
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
                            await mindmapStore.AddAsync(result.Name, result.Document);
                        }

                        await mindmapStore.LoadAsync(mindmapStore.AllMindmaps[0]);

                        SelectedMindmap = mindmapStore.AllMindmaps[0];
                    }
                }
                catch
                {
                    string content = ResourceManager.GetString("ImportFailed_Content");
                    string heading = ResourceManager.GetString("ImportFailed_Heading");

                    await MessageDialogService.AlertAsync(content, heading);
                }
            }).Forget();
        }

        private void MindmapStore_DocumentLoaded(object sender, DocumentLoadedEventArgs e)
        {
            if (e.Document == null)
            {
                SelectedMindmap = null;
            }
        }

        public async void OnSelectedMindmapChanged()
        {
            if (SelectedMindmap != null)
            {
                await mindmapStore.LoadAsync(SelectedMindmap);
            }
        }

        public bool IsValidMindmapName(string name)
        {
            return mindmapStore.IsValidMindmapName(name);
        }

        public async Task CreateNewMindmapAsync(string name)
        {
            await mindmapStore.CreateAsync(name);

            SelectedMindmap = mindmapStore.AllMindmaps.FirstOrDefault();
        }

        public async Task LoadAsync()
        {
            await mindmapStore.LoadAllAsync();

            SelectedMindmap = mindmapStore.AllMindmaps.FirstOrDefault();
        }
    }
}
