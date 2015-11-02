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
using Hercules.Model;
using Hercules.Model.Utils;
using Microsoft.Practices.Unity;
using PropertyChanged;

namespace Hercules.App.Modules.Mindmaps.ViewModels
{
    [ImplementPropertyChanged]
    public sealed class MindmapsViewModel : ViewModelBase
    {
        private RelayCommand<IMindmapRef> deleteCommand;

        public ObservableCollection<IMindmapRef> Mindmaps
        {
            get
            {
                return MindmapStore.AllMindmaps;
            }
        }

        [NotifyUI]
        public bool IsLoaded { get; set; }

        [NotifyUI]
        public IMindmapRef SelectedMindmap { get; set; }

        [Dependency]
        public IMindmapStore MindmapStore { get; set; }

        [Dependency]
        public IProcessManager ProcessManager { get; set; }

        [Dependency]
        public IMessageDialogService MessageDialogService { get; set; }

        public RelayCommand<IMindmapRef> DeleteCommand
        {
            get
            {
                return deleteCommand ?? (deleteCommand = new RelayCommand<IMindmapRef>(async item =>
                {
                    string content = ResourceManager.GetString("DeleteMindmap_Content");
                    string message = ResourceManager.GetString("DeleteMindmap_Title");

                    if (await MessageDialogService.ConfirmAsync(content, message))
                    {
                        await MindmapStore.DeleteAsync(item);
                    }
                }));
            }
        }

        public MindmapsViewModel()
        {
            MessengerInstance.Register<ImportMessage>(this, OnImport);
        }

        public void OnImport(ImportMessage message)
        {
            ImportAsync(message.Content);
        }

        private void ImportAsync(ImportModel model)
        {
            ProcessManager.RunMainProcessAsync(MindmapStore, async () =>
            {
                try
                {
                    List<KeyValuePair<string, Document>> documents = await model.Source.ImportAsync(model.Importer);

                    if (documents.Count > 0)
                    {
                        foreach (var document in documents)
                        {
                            await MindmapStore.AddAsync(document.Key, document.Value);
                        }

                        await MindmapStore.LoadAsync(MindmapStore.AllMindmaps[0]);
                    }
                }
                catch
                {
                    string content = ResourceManager.GetString("ImportFailed_Content");
                    string message = ResourceManager.GetString("ImportFailed_Title");

                    await MessageDialogService.AlertAsync(content, message);
                }
            }).Forget();
        }

        public async void OnSelectedMindmapChanged()
        {
            if (SelectedMindmap != null)
            {
                await MindmapStore.LoadAsync(SelectedMindmap);
            }
        }

        public async Task CreateNewMindmapAsync(string name)
        {
            await MindmapStore.CreateAsync(name);

            SelectedMindmap = MindmapStore.AllMindmaps.FirstOrDefault();
        }

        public async Task LoadAsync()
        {
            await MindmapStore.LoadAllAsync();

            SelectedMindmap = MindmapStore.AllMindmaps.FirstOrDefault();
        }
    }
}
