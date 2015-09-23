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
using System.Threading.Tasks;
using Windows.UI.Xaml;
using GP.Windows;
using Hercules.Model;
using Hercules.Model.Storing;

namespace Hercules.App.Components.Implementations
{
    public sealed class MindmapStore : IMindmapStore
    {
        private readonly ObservableCollection<IMindmapRef> allMindmaps = new ObservableCollection<IMindmapRef>();
        private readonly DispatcherTimer autosaveTimer = new DispatcherTimer();
        private readonly IDocumentStore documentStore;
        private readonly ILoadingManager loadingManager;
        private readonly ISettingsProvider settingsProvider;
        private readonly ILocalizationManager localizationManager;
        private Document loadedDocument;
        private IMindmapRef loadedMindmapRef;
        private bool isLoaded;

        public event EventHandler<DocumentLoadedEventArgs> DocumentLoaded;

        public ObservableCollection<IMindmapRef> AllMindmaps
        {
            get { return allMindmaps; }
        }

        public Document LoadedDocument
        {
            get { return loadedDocument; }
        }

        public MindmapStore(IDocumentStore documentStore, ILoadingManager loadingManager, ISettingsProvider settingsProvider, ILocalizationManager localizationManager)
        {
            Guard.NotNull(documentStore, nameof(documentStore));
            Guard.NotNull(loadingManager, nameof(loadingManager));
            Guard.NotNull(localizationManager, nameof(localizationManager));
            Guard.NotNull(settingsProvider, nameof(settingsProvider));

            this.documentStore = documentStore;
            this.loadingManager = loadingManager;
            this.localizationManager = localizationManager;
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

                await DoAsync(x => true, async () =>
                {
                    if (settingsProvider.IsAlreadyStarted)
                    {
                        IEnumerable<DocumentRef> documents = await documentStore.LoadAllAsync();

                        foreach (DocumentRef documentRef in documents)
                        {
                            MindmapRef mindmapRef = new MindmapRef(documentRef, documentStore);

                            allMindmaps.Add(mindmapRef);
                        }
                    }
                    else
                    {
                        settingsProvider.IsAlreadyStarted = true;

                        await CreateAsync(localizationManager.GetString("MyMindmap"));
                    }
                });
            }
        }

        public Task CreateAsync(string name)
        {
            return DoAsync(x => true, async () =>
            {
                Document document = new Document(Guid.NewGuid());

                RenameRoot(name, document);

                DocumentRef documentRef = await documentStore.CreateAsync(name, document);

                allMindmaps.Insert(0, new MindmapRef(documentRef, documentStore));
            });
        }

        private static void RenameRoot(string name, Document document)
        {
            string transactionName = ResourceManager.GetString("ChangeTextTransactionName");

            document.MakeTransaction(transactionName, commands =>
            {
                commands.Apply(new ChangeTextCommand(document.Root, name, true));
            });
        }

        public Task SaveAsync()
        {
            return DoAsync(x => x != null, async () =>
            {
                MindmapRef loadedRef = ValidateMindmap(loadedMindmapRef);

                await loadedRef.SaveAsync(loadedDocument);
            });
        }

        public Task LoadAsync(IMindmapRef mindmap)
        {
            MindmapRef mindmapRef = ValidateMindmap(mindmap);

            return this.DoAsync(x => x != mindmap, async () =>
            {
                MindmapRef loadedRef = loadedMindmapRef as MindmapRef;

                if (loadedRef != null)
                {
                    await loadedRef.SaveAsync(loadedDocument);
                }

                loadedMindmapRef = mindmap;
                loadedDocument = await documentStore.LoadAsync(mindmapRef.DocumentRef);

                OnDocumentLoaded(loadedDocument);
            });
        }

        public Task DeleteAsync(IMindmapRef mindmap)
        {
            MindmapRef mindmapRef = ValidateMindmap(mindmap);

            return DoAsync(x => true, async () =>
            {
                await mindmapRef.DeleteAsync();

                allMindmaps.Remove(mindmap);

                if (loadedMindmapRef == mindmap)
                {
                    loadedMindmapRef = null;
                    loadedDocument = null;

                    OnDocumentLoaded(null);
                }
            });
        }

        private MindmapRef ValidateMindmap(IMindmapRef mindmap)
        {
            Guard.NotNull(mindmap, nameof(mindmap));
            Guard.IsType<MindmapRef>(mindmap, nameof(mindmap));

            return mindmap as MindmapRef;
        }

        private async Task DoAsync(Predicate<IMindmapRef> predicate, Func<Task> action)
        {
            if (isLoaded && predicate(loadedMindmapRef))
            {
                loadingManager.BeginLoading();
                try
                {
                    await action();
                }
                finally
                {
                    loadingManager.FinishLoading();
                }
            }
        }

        private void OnDocumentLoaded(Document document)
        {
            DocumentLoaded?.Invoke(this, new DocumentLoadedEventArgs(document));
        }
    }
}
