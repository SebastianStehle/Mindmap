// ==========================================================================
// EditorViewModel.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Hercules.App.Components;
using Hercules.App.Components.Implementations;
using Hercules.App.Messages;
using Hercules.Model;
using Hercules.Model.Export;
using Hercules.Model.Rendering.Win2D;
using Hercules.Model.Rendering.Win2D.Default;
using Hercules.Model.Storing;
using Microsoft.Practices.Unity;
using PropertyChanged;

namespace Hercules.App.Modules.Editor.ViewModels
{
    [ImplementPropertyChanged]
    public sealed class EditorViewModel : ViewModelBase
    {
        private readonly DispatcherTimer autosaveTimer = new DispatcherTimer();
        private readonly IRendererFactory rendererFactory = new DefaultRendererFactory();
        private Document document;
        private MindmapItem mindmapItem;
        private RelayCommand redoCommand;
        private RelayCommand undoCommand;
        private RelayCommand removeCommand;
        private RelayCommand addChildCommand;
        private RelayCommand addSiblingCommand;
        private RelayCommand exportHtmlCommand;
        private RelayCommand exportImageCommand;

        [Dependency]
        public IDocumentStore DocumentStore { get; set; }

        [Dependency]
        public IOutlineGenerator OutlineGenerator { get; set; }

        [Dependency]
        public ILocalizationManager LocalizationManager { get; set; }

        [Dependency]
        public IMessageDialogService MessageDialogService { get; set; }

        public IRendererFactory RendererFactory
        {
            get
            {
                return rendererFactory;
            }
        }

        public Document Document
        {
            get
            {
                return document;
            }
            set
            {
                if (document != value)
                {
                    if (document != null)
                    {
                        document.UndoRedoManager.StateChanged -= UndoRedoManager_StateChanged;
                    }

                    document = value;

                    RaisePropertyChanged();

                    if (document != null)
                    {
                        document.UndoRedoManager.StateChanged += UndoRedoManager_StateChanged;
                    }
                }
            }
        }

        public RelayCommand RedoCommand
        {
            get
            {
                return redoCommand ?? (redoCommand = new RelayCommand(() =>
                {
                    if (Document.UndoRedoManager.CanRedo)
                    {
                        Document.UndoRedoManager.Redo();
                    }
                }, () => Document != null && Document.UndoRedoManager.CanRedo));
            }
        }

        public RelayCommand UndoCommand
        {
            get
            {
                return undoCommand ?? (undoCommand = new RelayCommand(() =>
                {
                    if (Document.UndoRedoManager.CanUndo)
                    {
                        Document.UndoRedoManager.Undo();
                    }
                }, () => Document != null && Document.UndoRedoManager.CanUndo));
            }
        }

        public RelayCommand ExportImageCommand
        {
            get
            {
                return exportImageCommand ?? (exportImageCommand = new RelayCommand(async () =>
                {
                    await MessageDialogService.SaveFileDialogAsync(new string[] { ".png" }, s => RendererFactory.Current.RenderScreenshotAsync(s, Colors.Transparent, 300));
                }, () => Document != null));
            }
        }

        public RelayCommand ExportHtmlCommand
        {
            get
            {
                return exportHtmlCommand ?? (exportHtmlCommand = new RelayCommand(async () =>
                {
                    string noText = LocalizationManager.GetString("NoText");

                    await MessageDialogService.SaveFileDialogAsync(new string[] { ".html" }, s => OutlineGenerator.GenerateOutline(Document, RendererFactory.Current, s, true, noText));
                }, () => Document != null));
            }
        }

        public RelayCommand RemoveCommand
        {
            get
            {
                return removeCommand ?? (removeCommand = new RelayCommand(() =>
                {
                    Node selectedNormalNode = Document.SelectedNode as Node;

                    if (selectedNormalNode != null)
                    {
                        string tansactionName = ResourceManager.GetString("RemoveNodeTransactionName");

                        Document.MakeTransaction(tansactionName, commands =>
                        {
                            commands.Apply(new RemoveChildCommand(selectedNormalNode.Parent, selectedNormalNode));
                        });
                    }
                }));
            }
        }

        public RelayCommand AddChildCommand
        {
            get
            {
                return addChildCommand ?? (addChildCommand = new RelayCommand(() =>
                {
                    NodeBase selectedNode = Document.SelectedNode;

                    if (selectedNode != null)
                    {
                        string tansactionName = ResourceManager.GetString("AddChildTransactionName");

                        Document.MakeTransaction(tansactionName, commands =>
                        {
                            commands.Apply(new InsertChildCommand(selectedNode, null, NodeSide.Undefined));
                        });
                    }
                }));
            }
        }

        public RelayCommand AddSiblingCommand
        {
            get
            {
                return addSiblingCommand ?? (addSiblingCommand = new RelayCommand(() =>
                {
                    Node selectedNormalNode = Document.SelectedNode as Node;

                    if (selectedNormalNode != null)
                    {
                        string tansactionName = ResourceManager.GetString("AddSiblingTransactionName");

                        Document.MakeTransaction(tansactionName, commands =>
                        {
                            commands.Apply(new InsertChildCommand(selectedNormalNode.Parent, null, selectedNormalNode.NodeSide));
                        });
                    }
                }));
            }
        }
        public EditorViewModel()
        {
            Messenger.Default.Register<MindmapDeletedMessage>(this, OnMindmapDeleted);
            Messenger.Default.Register<OpenMindmapMessage>(this, OnOpenMindmap);

            StartTimer();
        }

        private void StartTimer()
        {
            autosaveTimer.Interval = TimeSpan.FromMinutes(5);
            autosaveTimer.Tick += autosaveTimer_Tick;
            autosaveTimer.Start();
        }

        private void UndoRedoManager_StateChanged(object sender, EventArgs e)
        {
            UndoCommand.RaiseCanExecuteChanged();
            RedoCommand.RaiseCanExecuteChanged();
        }

        private void OnMindmapDeleted(MindmapDeletedMessage message)
        {
            if (message.Content == mindmapItem)
            {
                Document = null;

                mindmapItem = null;
            }
        }

        public async void OnOpenMindmap(OpenMindmapMessage message)
        {
            await SaveAsync();

            await LoadAsync(message.Content);

            UndoCommand.RaiseCanExecuteChanged();
            RedoCommand.RaiseCanExecuteChanged();
        }

        private async void autosaveTimer_Tick(object sender, object e)
        {
            await SaveAsync();
        }

        private async Task SaveAsync()
        {
            if (Document != null)
            {
                await DocumentStore.StoreAsync(Document);

                mindmapItem.RefreshAfterSave();
            }
        }

        public async Task LoadAsync(MindmapItem mindmapToLoad)
        {
            if (mindmapToLoad != null && (mindmapItem == null || mindmapItem.DocumentId != mindmapToLoad.DocumentId))
            {
                Document = await DocumentStore.LoadAsync(mindmapToLoad.DocumentId);

                mindmapItem = mindmapToLoad;
            }
        }
    }
}
