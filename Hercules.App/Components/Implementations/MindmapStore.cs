// ==========================================================================
// MindmapStore.cs
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
using Windows.UI.Xaml;
using GalaSoft.MvvmLight;
using GP.Windows;
using GP.Windows.Mvvm;
using Hercules.Model;
using Hercules.Model.Storing;
using Hercules.Model.Utils;
using PropertyChanged;

// ReSharper disable ImplicitlyCapturedClosure

namespace Hercules.App.Components.Implementations
{
    [ImplementPropertyChanged]
    public sealed class MindmapStore : ViewModelBase, IMindmapStore
    {
        private readonly ObservableCollection<MindmapRef> allMindmaps = new ObservableCollection<MindmapRef>();
        private readonly DispatcherTimer autosaveTimer = new DispatcherTimer();
        private readonly IDocumentStore documentStore;
        private readonly IProcessManager processManager;
        private readonly ISettingsProvider settingsProvider;
        private readonly IMessageDialogService dialogService;
        private bool isLoaded;

        public event EventHandler<DocumentLoadedEventArgs> DocumentLoaded;

        public ObservableCollection<MindmapRef> AllMindmaps
        {
            get { return allMindmaps; }
        }

        [NotifyUI]
        public Document LoadedDocument { get; private set; }

        [NotifyUI]
        public MindmapRef LoadedMindmap { get; private set; }

        public MindmapStore(IDocumentStore documentStore, IProcessManager processManager, ISettingsProvider settingsProvider, IMessageDialogService dialogService)
        {
            Guard.NotNull(documentStore, nameof(documentStore));
            Guard.NotNull(dialogService, nameof(dialogService));
            Guard.NotNull(processManager, nameof(processManager));
            Guard.NotNull(settingsProvider, nameof(settingsProvider));

            this.dialogService = dialogService;
            this.documentStore = documentStore;
            this.processManager = processManager;
            this.settingsProvider = settingsProvider;

            StartTimer();
        }

        private void StartTimer()
        {
            autosaveTimer.Interval = TimeSpan.FromMinutes(5);
            autosaveTimer.Tick += autosaveTimer_Tick;
            autosaveTimer.Start();
        }

        private async void autosaveTimer_Tick(object sender, object e)
        {
            await SaveAsync();
        }

        public async Task LoadAllAsync()
        {
            if (!isLoaded)
            {
                isLoaded = true;

                await DoAsync(async () =>
                {
                    if (settingsProvider.IsAlreadyStarted)
                    {
                        IEnumerable<DocumentRef> documents = await documentStore.LoadAllAsync();

                        foreach (MindmapRef mindmapRef in documents.Select(documentRef => new MindmapRef(documentRef, this)))
                        {
                            allMindmaps.Add(mindmapRef);
                        }
                    }
                    else
                    {
                        settingsProvider.IsAlreadyStarted = true;

                        await CreateAsync(ResourceManager.GetString("MyMindmap"));
                    }
                });
            }
        }

        public Task CreateAsync(string name)
        {
            Guard.NotNullOrEmpty(name, nameof(name));

            return DoAsync(async () =>
            {
                Document document = new Document(Guid.NewGuid());

                document.Root.ChangeTextTransactional(name);

                await AddAsync(name, document);
            });
        }

        public Task AddAsync(string name, Document document)
        {
            Guard.NotNullOrEmpty(name, nameof(name));
            Guard.NotNull(document, nameof(document));

            return DoAsync(async() =>
            {
                DocumentRef documentRef = await documentStore.CreateAsync(name, document);

                allMindmaps.Insert(0, new MindmapRef(documentRef, this));
            });
        }

        public Task SaveAsync()
        {
            return DoAsync(LoadedMindmap, async () =>
            {
                await documentStore.StoreAsync(LoadedMindmap.DocumentRef, LoadedDocument);
            });
        }

        public Task RenameAsync(MindmapRef mindmap, string newName)
        {
            Guard.NotNull(mindmap, nameof(mindmap));

            return DoAsync(mindmap, async () =>
            {
                await documentStore.RenameAsync(mindmap.DocumentRef, newName);
            });
        }

        public Task SaveAsync(MindmapRef mindmap, Document document)
        {
            Guard.NotNull(mindmap, nameof(mindmap));

            return DoAsync(mindmap, async () =>
            {
                await documentStore.StoreAsync(mindmap.DocumentRef, document);
            });
        }

        public Task LoadAsync(MindmapRef mindmap)
        {
            Guard.NotNull(mindmap, nameof(mindmap));

            return DoAsync(mindmap, async () =>
            {
                LoadedMindmap = mindmap;
                LoadedDocument = await documentStore.LoadAsync(mindmap.DocumentRef);

                OnDocumentLoaded(LoadedDocument);
            });
        }

        public Task DeleteAsync(MindmapRef mindmap)
        {
            Guard.NotNull(mindmap, nameof(mindmap));

            return DoAsync(mindmap, async () =>
            {
                if (mindmap.DocumentRef != null)
                {
                    await documentStore.DeleteAsync(mindmap.DocumentRef);
                }

                UnloadMindmap(mindmap);
            });
        }

        private Task DoAsync(MindmapRef mindmap, Func<Task> action)
        {
            return DoAsync(mindmap, x => true, action);
        }

        private async Task DoAsync(Func<Task> action)
        {
            if (isLoaded)
            {
                try
                {
                    await processManager.RunMainProcessAsync(this, action);
                }
                catch (DocumentNotFoundException)
                {
                    ShowErrorDialog();
                }
            }
        }

        private async Task DoAsync(MindmapRef mindmap, Predicate<MindmapRef> predicate, Func<Task> action)
        {
            if (isLoaded && mindmap != null && predicate(mindmap))
            {
                try
                {
                    await processManager.RunMainProcessAsync(this, action);
                }
                catch (DocumentNotFoundException)
                {
                    ShowErrorDialog();

                    UnloadMindmap(mindmap);
                }
                finally
                {
                    mindmap.RefreshProperties();
                }
            }
        }

        private void ShowErrorDialog()
        {
            string content = ResourceManager.GetString("MindmapDeleted_Content");
            string heading = ResourceManager.GetString("MindmapDeleted_Heading");

            dialogService.AlertAsync(content, heading).Forget();
        }

        private void UnloadMindmap(MindmapRef mindmap)
        {
            allMindmaps.Remove(mindmap);

            if (LoadedMindmap == mindmap)
            {
                LoadedMindmap = null;
                LoadedDocument = null;

                OnDocumentLoaded(null);
            }
        }

        private void OnDocumentLoaded(Document document)
        {
            DocumentLoaded?.Invoke(this, new DocumentLoadedEventArgs(document));
        }
    }
}
