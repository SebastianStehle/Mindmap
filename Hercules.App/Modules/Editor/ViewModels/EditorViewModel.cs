// ==========================================================================
// EditorViewModel.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System;
using Windows.UI;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using GP.Windows;
using Hercules.App.Components;
using Hercules.App.Messages;
using Hercules.Model;
using Hercules.Model.Export;
using Hercules.Model.Rendering.Win2D;
using Hercules.Model.Rendering.Win2D.Default;
using Hercules.Model.Storing;
using Hercules.Model.Utils;
using Microsoft.Practices.Unity;
using PropertyChanged;

namespace Hercules.App.Modules.Editor.ViewModels
{
    [ImplementPropertyChanged]
    public sealed class EditorViewModel : ViewModelBase
    {
        private readonly IRendererFactory rendererFactory = new DefaultRendererFactory();
        private readonly IMindmapStore mindmapStore;
        private Document document;
        private RelayCommand redoCommand;
        private RelayCommand undoCommand;
        private RelayCommand removeCommand;
        private RelayCommand addChildCommand;
        private RelayCommand addSiblingCommand;
        private RelayCommand exportHtmlCommand;
        private RelayCommand exportImageCommand;
        private RelayCommand selectTopCommand;
        private RelayCommand selectLeftCommand;
        private RelayCommand selectRightCommand;
        private RelayCommand selectBottomCommand;

        [NotifyUI]
        public bool ShowHelp { get; set; }

        [Dependency]
        public IDocumentStore DocumentStore { get; set; }

        [Dependency]
        public IOutlineGenerator OutlineGenerator { get; set; }

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
                    await MessageDialogService.SaveFileDialogAsync(new string[] { ".png" }, s => RendererFactory.Current.RenderScreenshotAsync(s, Colors.White, 300));
                }, () => Document != null));
            }
        }

        public RelayCommand ExportHtmlCommand
        {
            get
            {
                return exportHtmlCommand ?? (exportHtmlCommand = new RelayCommand(async () =>
                {
                    string noText = ResourceManager.GetString("Outline_NoText");

                    await MessageDialogService.SaveFileDialogAsync(new string[] { ".html" }, s => OutlineGenerator.GenerateOutline(Document, RendererFactory.Current, s, true, noText));
                }, () => Document != null));
            }
        }

        public RelayCommand SelectTopCommand
        {
            get
            {
                return selectTopCommand ?? (selectTopCommand = new RelayCommand(() =>
                {
                    Document.SelectTopOfSelectedNode();
                }));
            }
        }

        public RelayCommand SelectRightCommand
        {
            get
            {
                return selectRightCommand ?? (selectRightCommand = new RelayCommand(() =>
                {
                    Document.SelectRightOfSelectedNode();
                }));
            }
        }

        public RelayCommand SelectBottomCommand
        {
            get
            {
                return selectBottomCommand ?? (selectBottomCommand = new RelayCommand(() =>
                {
                    Document.SelectBottomOfSelectedNode();
                }));
            }
        }

        public RelayCommand SelectLeftCommand
        {
            get
            {
                return selectLeftCommand ?? (selectLeftCommand = new RelayCommand(() =>
                {
                    Document.SelectLeftOfSelectedNode();
                }));
            }
        }

        public RelayCommand RemoveCommand
        {
            get
            {
                return removeCommand ?? (removeCommand = new RelayCommand(() =>
                {
                    Document.RemoveSelectedNodeTransactional();
                }));
            }
        }

        public RelayCommand AddChildCommand
        {
            get
            {
                return addChildCommand ?? (addChildCommand = new RelayCommand(() =>
                {
                    Document.AddChildToSelectedNodeTransactional();
                }));
            }
        }

        public RelayCommand AddSiblingCommand
        {
            get
            {
                return addSiblingCommand ?? (addSiblingCommand = new RelayCommand(() =>
                {
                    Document.AddSibilingToSelectedNodeTransactional();
                }));
            }
        }

        public EditorViewModel()
        {
            Messenger.Default.Register<SaveMindmapMessage>(this, OnSaveMindmap);
        }

        public EditorViewModel(IMindmapStore mindmapStore)
            : this()
        {
            this.mindmapStore = mindmapStore;

            mindmapStore.DocumentLoaded += MindmapStore_DocumentLoaded;
        }

        private void MindmapStore_DocumentLoaded(object sender, DocumentLoadedEventArgs e)
        {
            Document = e.Document;

            UpdateUndoRedo();
        }

        private async void OnSaveMindmap(SaveMindmapMessage message)
        {
            await mindmapStore.SaveAsync();

            message.Callback();
        }

        private void UndoRedoManager_StateChanged(object sender, EventArgs e)
        {
            UpdateUndoRedo();
        }

        private void UpdateUndoRedo()
        {
            UndoCommand.RaiseCanExecuteChanged();
            RedoCommand.RaiseCanExecuteChanged();
        }
    }
}
