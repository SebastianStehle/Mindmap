// ==========================================================================
// MindmapsViewModel.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GP.Windows;
using Hercules.App.Components;
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
        public IMessageDialogService MessageDialogService { get; set; }

        public RelayCommand<IMindmapRef> DeleteCommand
        {
            get
            {
                return deleteCommand ?? (deleteCommand = new RelayCommand<IMindmapRef>(async item =>
                {
                    string content = ResourceManager.GetString("DeleteMindmap_Content"),
                             title = ResourceManager.GetString("DeleteMindmap_Title");

                    if (await MessageDialogService.ConfirmAsync(content, title))
                    {
                        await MindmapStore.DeleteAsync(item);
                    }
                }));
            }
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
