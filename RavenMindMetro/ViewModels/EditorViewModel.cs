// ==========================================================================
// MainViewModel.cs
// RavenMind Application
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using RavenMind.Messages;
using RavenMind.Model;
using System;
using System.Collections.Generic;
using System.Composition;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace RavenMind.ViewModels
{
    [Export]
    [Export(typeof(ViewModelBase))]
    public sealed class EditorViewModel : ViewModelBase
    {
        private readonly DispatcherTimer autosaveTimer = new DispatcherTimer();
        private Document document;
        private RelayCommand redoCommand;
        private RelayCommand undoCommand;
        private RelayCommand removeCommand;
        private RelayCommand addChildCommand;
        private RelayCommand addSiblingCommand;

        [Import]
        public IDocumentStore DocumentStore { get; set; }

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

                    RaisePropertyChanged("Document");

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
                            Document.MakeTransaction("RemoveNode", c =>
                            {
                                c.Apply(new RemoveChildCommand(selectedNormalNode.Parent, selectedNormalNode));
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
                        Document.MakeTransaction("AddChild", c =>
                        {
                            c.Apply(new InsertChildCommand(selectedNode, null, NodeSide.Undefined));
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
                        Document.MakeTransaction("AddSibling", c =>
                        {
                            c.Apply(new InsertChildCommand(selectedNormalNode.Parent, null, selectedNormalNode.NodeSide));
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

            Document = new Document(Guid.NewGuid(), "NewMindmap");
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
                    finalId = first.Id;
                }
            }

            if (finalId != null)
            {
                Document = await DocumentStore.LoadAsync(finalId.Value);
            }
        }
    }
}
