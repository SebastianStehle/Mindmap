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
using RavenMind.Assets;
using RavenMind.Messages;
using RavenMind.Model;
using System;
using System.Collections.Generic;
using System.Composition;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace RavenMind.ViewModels
{
    [Export]
    [Export(typeof(ViewModelBase))]
    public sealed class EditorViewModel : ViewModelBase
    {
        #region Fields

        private readonly DispatcherTimer autosaveTimer = new DispatcherTimer();

        #endregion

        #region Properties

        [Import]
        public IDocumentStore DocumentStore { get; set; }

        private NodeBase selectedNode;
        public NodeBase SelectedNode
        {
            get
            {
                return selectedNode;
            }
            set
            {
                if (selectedNode != value)
                {
                    selectedNode = value;
                    RaisePropertyChanged("SelectedNode");
                }
            }
        }

        private Document document;
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
                        document.SelectionChanged -= document_SelectionChanged;
                        document.UndoRedoManager.StateChanged -= UndoRedoManager_StateChanged;
                    }

                    document = value;

                    RaisePropertyChanged("Document");

                    if (document != null)
                    {
                        document.SelectionChanged += document_SelectionChanged;
                        document.UndoRedoManager.StateChanged += UndoRedoManager_StateChanged;
                    }

                    SelectedNode = null;
                }
            }
        }

        #endregion

        #region Commands

        private RelayCommand redoCommand;
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

        private RelayCommand undoCommand;
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

        private RelayCommand<string> changeColorCommand;
        public RelayCommand<string> ChangeColorCommand
        {
            get
            {
                return changeColorCommand ?? (changeColorCommand = new RelayCommand<string>(x =>
                {
                    if (SelectedNode != null)
                    {
                        Document.BeginTransaction("ChangeColor");
                        Document.Apply(new ChangeColorCommand { Node = SelectedNode, Color = int.Parse(x, NumberStyles.HexNumber, CultureInfo.InvariantCulture) });
                        Document.CommitTransaction();
                    }
                }));
            }
        }

        private RelayCommand removeCommand;
        public RelayCommand RemoveCommand
        {
            get
            {
                return removeCommand ?? (removeCommand = new RelayCommand(() =>
                {
                    if (SelectedNode != null)
                    {
                        Node selectedNormalNode = Document.SelectedNode as Node;

                        if (selectedNormalNode != null)
                        {
                            NodeBase parent = selectedNormalNode.Parent;

                            Document.BeginTransaction("Remove Node");
                            Document.Apply(new RemoveChildCommand { Node = selectedNode.Parent, OldNode = selectedNormalNode });
                            Document.CommitTransaction();

                            parent.IsSelected = true;
                        }
                    }
                }));
            }
        }

        private RelayCommand addChildCommand;
        public RelayCommand AddChildCommand
        {
            get
            {
                return addChildCommand ?? (addChildCommand = new RelayCommand(() =>
                {
                    if (SelectedNode != null)
                    {
                        Document.BeginTransaction("AddChild");
                        Document.Apply(new InsertChildCommand { Node = SelectedNode, NewNode = new Node(Guid.NewGuid()) });
                        Document.CommitTransaction();
                    }
                }));
            }
        }

        private RelayCommand addSiblingCommand;
        public RelayCommand AddSiblingCommand
        {
            get
            {
                return addSiblingCommand ?? (addSiblingCommand = new RelayCommand(() =>
                {
                    Node normaleNode = SelectedNode as Node;

                    if (normaleNode != null)
                    {
                        Document.BeginTransaction("AddSibling");
                        Document.Apply(new InsertChildCommand { Node = SelectedNode.Parent, NewNode = new Node(Guid.NewGuid()), Side = SelectedNode.Side });
                        Document.CommitTransaction();
                    }
                }));
            }
        }

        #endregion

        #region Constructors

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

        #endregion

        #region Methods

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

        private void document_SelectionChanged(object sender, EventArgs e)
        {
            SelectedNode = Document.SelectedNode;
        }

        #endregion
    }
}
