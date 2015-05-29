// ==========================================================================
// MindmapsViewModel.cs
// Mindmap Application
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.Practices.Unity;
using MindmapApp.Components;
using MindmapApp.Messages;
using MindmapApp.Model;
using MindmapApp.Model.Storing;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace MindmapApp.ViewModels
{
    public sealed class MindmapsViewModel : ViewModelBase
    {
        private readonly ObservableCollection<MindmapItem> mindmaps = new ObservableCollection<MindmapItem>();
        private bool isLoaded;
        private MindmapItem selectedMindmap;

        [Dependency]
        public IDocumentStore DocumentStore { get; set; }

        [Dependency]
        public ILocalizationManager LocalizationManager { get; set; }

        [Dependency]
        public ISettingsProvider SettingsProvider { get; set; }

        public ObservableCollection<MindmapItem> Mindmaps
        {
            get
            {
                return mindmaps;
            }
        }

        public MindmapItem SelectedMindmap
        {
            get
            {
                return selectedMindmap;
            }
            set
            {
                if (selectedMindmap != value)
                {
                    selectedMindmap = value;
                    RaisePropertyChanged("SelectedMindmap");

                    if (isLoaded && selectedMindmap != null)
                    {
                        Messenger.Default.Send(new OpenMindmapMessage(selectedMindmap.MindmapId));
                    }
                }
            }
        }

        public MindmapsViewModel()
        {
            Messenger.Default.Register<DeleteMindmapMessage>(this, OnDeleteMindmap);
            Messenger.Default.Register<OpenMindmapMessage>(this, OnOpenMindmap);
            Messenger.Default.Register<NameChangedMessage>(this, OnNameChanged);
            Messenger.Default.Register<MindmapSavedMessage>(this, OnMindmapSaved);
        }

        public void OnMindmapSaved(MindmapSavedMessage message)
        {
            MindmapItem item = Mindmaps.FirstOrDefault(x => x.MindmapId == message.Content);

            if (item != null)
            {
                item.LastUpdate = DateTime.Now.ToString("g", CultureInfo.CurrentCulture);
            }
        }

        public void OnNameChanged(NameChangedMessage message)
        {
            if (SelectedMindmap != null)
            {
                SelectedMindmap.Name = message.Content;
            }
        }

        public void OnOpenMindmap(OpenMindmapMessage message)
        {
            Func<MindmapItem, bool> predicate = x => true;

            if (message.Content != null)
            {
                predicate = x => x.MindmapId == message.Content.Value;
            }

            SelectedMindmap = Mindmaps.FirstOrDefault(predicate);

            if (SelectedMindmap == null)
            {
                SelectedMindmap = Mindmaps.FirstOrDefault();
            }
        }

        public async void OnDeleteMindmap(DeleteMindmapMessage message)
        {
            await DocumentStore.DeleteAsync(message.Content);

            Mindmaps.Remove(Mindmaps.Single(x => x.MindmapId == message.Content));
        }

        public async Task CreateNewMindmapAsync(string name, string text)
        {
            Document document = new Document(Guid.NewGuid(), name);

            DocumentRef documentRef = await DocumentStore.StoreAsync(document);

            Mindmaps.Insert(0, CreateMindmapItem(name, documentRef));

            SelectedMindmap = Mindmaps.FirstOrDefault();
        }

        public async Task LoadAsync()
        {
            if (!isLoaded)
            {
                isLoaded = true;

                if (SettingsProvider.IsAlreadyStarted)
                {
                    IEnumerable<DocumentRef> documents = await DocumentStore.LoadAllAsync();

                    foreach (DocumentRef documentRef in documents)
                    {
                        if (!Mindmaps.Any(x => x.MindmapId == documentRef.Id))
                        {
                            MindmapItem mindmapItem = CreateMindmapItem(documentRef.Name, documentRef);

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

        private static MindmapItem CreateMindmapItem(string name, DocumentRef documentRef)
        {
            MindmapItem mindmapItem = new MindmapItem();
            mindmapItem.Name = name;
            mindmapItem.LastUpdate = documentRef.LastUpdate.ToString("g", CultureInfo.CurrentCulture);
            mindmapItem.MindmapId = documentRef.Id;

            return mindmapItem;
        }
    }
}
