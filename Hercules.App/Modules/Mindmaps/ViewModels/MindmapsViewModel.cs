// ==========================================================================
// MindmapsViewModel.cs
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
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using GP.Windows;
using Hercules.App.Components;
using Hercules.App.Messages;
using Hercules.Model;
using Hercules.Model.Storing;
using Microsoft.Practices.Unity;
using PropertyChanged;

namespace Hercules.App.Modules.Mindmaps.ViewModels
{
    [ImplementPropertyChanged]
    public sealed class MindmapsViewModel : ViewModelBase
    {
        private readonly ObservableCollection<MindmapItem> mindmaps = new ObservableCollection<MindmapItem>();
        private RelayCommand<MindmapItem> deleteCommand;

        public ObservableCollection<MindmapItem> Mindmaps
        {
            get
            {
                return mindmaps;
            }
        }

        [NotifyUI]
        public MindmapItem SelectedMindmap { get; set; }

        [NotifyUI]
        public bool IsLoaded { get; set; }

        [Dependency]
        public IDocumentStore DocumentStore { get; set; }

        [Dependency]
        public ISettingsProvider SettingsProvider { get; set; }

        [Dependency]
        public ILocalizationManager LocalizationManager { get; set; }

        [Dependency]
        public IMessageDialogService MessageDialogService { get; set; }

        public RelayCommand<MindmapItem> DeleteCommand
        {
            get
            {
                return deleteCommand ?? (deleteCommand = new RelayCommand<MindmapItem>(async item =>
                {
                    string content = LocalizationManager.GetString("DeleteMindmapContent"),
                             title = LocalizationManager.GetString("DeleteMindmapTitle");

                    if (await MessageDialogService.ConfirmAsync(content, title))
                    {
                        await item.RemoveAsync();

                        Mindmaps.Remove(item);
                    }
                }));
            }
        }

        public void OnSelectedMindmapChanged()
        {
            if (IsLoaded && SelectedMindmap != null)
            {
                Messenger.Default.Send(new OpenMindmapMessage(SelectedMindmap));
            }
        }

        public async Task CreateNewMindmapAsync(string name, string text)
        {
            if (IsLoaded)
            {
                DocumentRef documentRef = await CreateMindmapAsync(name);

                AddMindmap(documentRef);
            }
        }

        private async Task<DocumentRef> CreateMindmapAsync(string name)
        {
            Document document = new Document(Guid.NewGuid(), name);

            DocumentRef documentRef = await DocumentStore.StoreAsync(document);

            return documentRef;
        }

        private void AddMindmap(DocumentRef documentRef)
        {
            Mindmaps.Insert(0, new MindmapItem(documentRef, DocumentStore));

            SelectedMindmap = Mindmaps.FirstOrDefault();
        }

        public async Task LoadAsync()
        {
            if (!IsLoaded)
            {
                IsLoaded = true;

                if (SettingsProvider.IsAlreadyStarted)
                {
                    IEnumerable<DocumentRef> documents = await DocumentStore.LoadAllAsync();

                    foreach (DocumentRef documentRef in documents)
                    {
                        if (Mindmaps.All(x => x.DocumentId != documentRef.DocumentId))
                        {
                            MindmapItem mindmapItem = new MindmapItem(documentRef, DocumentStore);

                            Mindmaps.Add(mindmapItem);
                        }
                    }

                    SelectedMindmap = Mindmaps.FirstOrDefault();
                }
                else
                {
                    SettingsProvider.IsAlreadyStarted = true;

                    await CreateNewMindmapAsync(LocalizationManager.GetString("MyMindmap"), LocalizationManager.GetString("Start"));
                }
            }
        }
    }
}
