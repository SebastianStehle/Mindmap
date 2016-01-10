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
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using GalaSoft.MvvmLight;
using GP.Windows;
using GP.Windows.Mvvm;
using Hercules.Model;
using Hercules.Model.Storing;
using Hercules.Model.Utils;
using Microsoft.ApplicationInsights.DataContracts;
using PropertyChanged;

// ReSharper disable ImplicitlyCapturedClosure

namespace Hercules.App.Components.Implementations
{
    [ImplementPropertyChanged]
    public sealed class MindmapStore : ViewModelBase, IMindmapStore
    {
        private readonly ObservableCollection<MindmapRef> allMindmaps = new ObservableCollection<MindmapRef>();
        private readonly DispatcherTimer autosaveTimer = new DispatcherTimer { Interval = TimeSpan.FromMinutes(5) };
        private readonly IDocumentStore documentStore;
        private readonly IProcessManager processManager;
        private readonly ISettingsProvider settingsProvider;
        private readonly IMessageDialogService dialogService;
        private bool isLoaded;

        public event EventHandler<DocumentLoadedEventArgs> DocumentLoaded;

        public event EventHandler MindmapUpdated;

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
            autosaveTimer.Tick += autosaveTimer_Tick;

            autosaveTimer.Start();
        }

        private void autosaveTimer_Tick(object sender, object e)
        {
            if (LoadedMindmap != null && LoadedDocument != null)
            {
                documentStore.StoreAsync(LoadedMindmap.DocumentRef, LoadedDocument).Forget();
            }
        }

        public bool IsValidMindmapName(string name)
        {
            return !string.IsNullOrWhiteSpace(name) && name == name.Trim() && !name.Intersect(Path.GetInvalidFileNameChars()).Any();
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
            Guard.ValidFileName(name, nameof(name));

            return DoAsync(async () =>
            {
                Document document = new Document(Guid.NewGuid());

                document.Root.ChangeTextTransactional(name);

                await AddAsync(name, document);
            });
        }

        public Task AddAsync(string name, Document document)
        {
            Guard.ValidFileName(name, nameof(name));
            Guard.NotNull(document, nameof(document));

            return DoAsync(async () =>
            {
                DocumentRef documentRef = await documentStore.CreateAsync(name, document);

                allMindmaps.Insert(0, new MindmapRef(documentRef, this));
            });
        }

        public Task SaveAsync()
        {
            return DoAsync(LoadedMindmap, LoadedDocument, async (m, d) =>
            {
                await documentStore.StoreAsync(m.DocumentRef, d);
            });
        }

        public Task RenameAsync(MindmapRef mindmap, string newName)
        {
            Guard.NotNull(mindmap, nameof(mindmap));
            Guard.ValidFileName(newName, nameof(newName));

            return DoAsync(mindmap, async m =>
            {
                await documentStore.RenameAsync(m.DocumentRef, newName);

                OnMindmapUpdated();
            });
        }

        public Task SaveAsync(MindmapRef mindmap, Document document)
        {
            Guard.NotNull(mindmap, nameof(mindmap));
            Guard.NotNull(document, nameof(document));

            return DoAsync(mindmap, document, async (m, d) =>
            {
                await documentStore.StoreAsync(m.DocumentRef, d);

                OnMindmapUpdated();
            });
        }

        public Task LoadAsync(MindmapRef mindmap)
        {
            Guard.NotNull(mindmap, nameof(mindmap));

            return DoAsync(mindmap, async m =>
            {
                if (LoadedMindmap != null && LoadedDocument != null)
                {
                    await LoadedMindmap.SaveAsync(LoadedDocument);
                }

                try
                {
                    LoadedMindmap = m;
                    LoadedDocument = await documentStore.LoadAsync(m.DocumentRef);

                    OnDocumentLoaded(LoadedDocument);
                }
                catch (DocumentNotFoundException e)
                {
                    ShowNotFoundErrorDialog(e);

                    UnloadMindmap(mindmap);
                }
                catch (Exception e)
                {
                    UnloadMindmap(m);

                    ShowLoadingErrorDialog(e);
                }
            });
        }

        public Task DeleteAsync(MindmapRef mindmap)
        {
            Guard.NotNull(mindmap, nameof(mindmap));

            return DoAsync(mindmap, async m =>
            {
                await documentStore.DeleteAsync(m.DocumentRef);

                UnloadMindmap(m);
            });
        }

        private async Task DoAsync(Func<Task> action)
        {
            if (isLoaded)
            {
                try
                {
                    await processManager.RunMainProcessAsync(this, action);
                }
                catch (DocumentNotFoundException e)
                {
                    ShowNotFoundErrorDialog(e);
                }
            }
        }

        private async Task DoAsync(MindmapRef mindmap, Func<MindmapRef, Task> action)
        {
            if (isLoaded && mindmap?.DocumentRef != null)
            {
                try
                {
                    await processManager.RunMainProcessAsync(this, () => action(mindmap));
                }
                catch (DocumentNotFoundException e)
                {
                    ShowNotFoundErrorDialog(e);

                    UnloadMindmap(mindmap);
                }
                finally
                {
                    mindmap.RefreshProperties();
                }
            }
        }

        private async Task DoAsync(MindmapRef mindmap, Document document, Func<MindmapRef, Document, Task> action)
        {
            if (isLoaded && mindmap?.DocumentRef != null && document != null)
            {
                try
                {
                    await processManager.RunMainProcessAsync(this, () => action(mindmap, document));
                }
                catch (DocumentNotFoundException e)
                {
                    ShowNotFoundErrorDialog(e);

                    UnloadMindmap(mindmap);
                }
                finally
                {
                    mindmap.RefreshProperties();
                }
            }
        }

        private void ShowLoadingErrorDialog(Exception e)
        {
            string content = ResourceManager.GetString("MindmapLoadingFailed_Content");
            string heading = ResourceManager.GetString("MindmapLoadingFailed_Heading");

            dialogService.AlertAsync(content, heading).Forget();

            App.TelemetryClient?.TrackException(new ExceptionTelemetry(e));
        }

        private void ShowNotFoundErrorDialog(Exception e)
        {
            string content = ResourceManager.GetString("MindmapDeleted_Content");
            string heading = ResourceManager.GetString("MindmapDeleted_Heading");

            dialogService.AlertAsync(content, heading).Forget();

            App.TelemetryClient?.TrackException(new ExceptionTelemetry(e));
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

        private void OnMindmapUpdated()
        {
            MindmapUpdated?.Invoke(this, EventArgs.Empty);
        }

        private void OnDocumentLoaded(Document document)
        {
            DocumentLoaded?.Invoke(this, new DocumentLoadedEventArgs(document));
        }
    }
}
