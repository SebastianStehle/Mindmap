// ==========================================================================
// MainViewModel.cs
// Mindmap Application
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.Practices.Unity;
using Hercules.App.Messages;
using Hercules.Model;
using Hercules.Model.Storing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Hercules.App.Components.Implementations;
using Hercules.Model.Rendering.Win2D;
using Hercules.Model.Rendering.Win2D.Default;
using PropertyChanged;

namespace Hercules.App.ViewModels
{
    [ImplementPropertyChanged]
    public sealed class EditorViewModel : ViewModelBase
    {
        private readonly DispatcherTimer autosaveTimer = new DispatcherTimer();
        private readonly Win2DRenderer renderer = new DefaultRenderer();
        private Document document;
        private RelayCommand redoCommand;
        private RelayCommand undoCommand;
        private RelayCommand removeCommand;
        private RelayCommand addChildCommand;
        private RelayCommand addSiblingCommand;

        [Dependency]
        public IDocumentStore DocumentStore { get; set; }

        public Win2DRenderer Renderer
        {
            get
            {
                return renderer;
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
                        string tansactionName = ResourceManager.GetString("AddSibilingTransactionName");

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
            autosaveTimer.Interval = TimeSpan.FromMinutes(5);
            autosaveTimer.Tick += autosaveTimer_Tick;
            autosaveTimer.Start();

            Messenger.Default.Register<SaveMindmapMessage>(this, OnSaveMindmap);
            Messenger.Default.Register<OpenMindmapMessage>(this, OnOpenMindmap);
            Messenger.Default.Register<DeleteMindmapMessage>(this, OnDeleteMindmap);
        }

        private void UndoRedoManager_StateChanged(object sender, EventArgs e)
        {
            UndoCommand.RaiseCanExecuteChanged();
            RedoCommand.RaiseCanExecuteChanged();
        }

        public void OnDeleteMindmap(DeleteMindmapMessage message)
        {
            Document = null;
        }

        public async void OnOpenMindmap(OpenMindmapMessage message)
        {
            await SaveAsync();
            await LoadAsync(message.Content);

            UndoCommand.RaiseCanExecuteChanged();
            RedoCommand.RaiseCanExecuteChanged();
        }

        public async void OnSaveMindmap(SaveMindmapMessage message)
        {
            try
            {
                await SaveAsync();
            }
            finally
            {
                message.Complete();
            }
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

                Messenger.Default.Send(new MindmapSavedMessage(Document.Id));
            }
        }

        public async Task LoadAsync(Guid? id)
        {
            Guid? finalId = id;

            if (finalId == null)
            {
                IEnumerable<DocumentRef> all = await DocumentStore.LoadAllAsync();

                DocumentRef first = all.FirstOrDefault();

                if (first != null)
                {
                    finalId = first.DocumentId;
                }
            }

            if (finalId != null && (Document == null || Document.Id != finalId))
            {
                Document = await DocumentStore.LoadAsync(finalId.Value);
            }
        }
    }
}
