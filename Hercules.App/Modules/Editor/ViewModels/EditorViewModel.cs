// ==========================================================================
// EditorViewModel.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using GP.Windows;
using GP.Windows.Mvvm;
using Hercules.App.Components;
using Hercules.App.Messages;
using Hercules.Model;
using Hercules.Model.ExImport;
using Hercules.Model.Storing;
using Hercules.Win2D.Rendering;
using Hercules.Win2D.Rendering.Themes.ModernPastel;
using Microsoft.Practices.Unity;
using PropertyChanged;

namespace Hercules.App.Modules.Editor.ViewModels
{
    [ImplementPropertyChanged]
    public sealed class EditorViewModel : ViewModelBase
    {
        private readonly IWin2DRendererProvider rendererProvider = new ModernPastelRendererProvider();
        private readonly IMindmapStore mindmapStore;
        private Document document;
        private RelayCommand<ExportModel> exportCommand;
        private RelayCommand<ImportModel> importCommand;
        private RelayCommand printCommand;
        private RelayCommand redoCommand;
        private RelayCommand undoCommand;
        private RelayCommand removeCommand;
        private RelayCommand addChildCommand;
        private RelayCommand addSiblingCommand;
        private RelayCommand selectTopCommand;
        private RelayCommand selectLeftCommand;
        private RelayCommand selectRightCommand;
        private RelayCommand selectBottomCommand;

        [NotifyUI]
        public bool ShowHelp { get; set; }

        [Dependency]
        public IPrintService PrintService { get; set; }

        [Dependency]
        public IDocumentStore DocumentStore { get; set; }

        [Dependency]
        public IOutlineGenerator OutlineGenerator { get; set; }

        [Dependency]
        public IExportTarget[] ExportTargets { get; set; }

        [Dependency]
        public IExporter[] Exporters { get; set; }

        [Dependency]
        public IImportSource[] IImportSources { get; set; }

        [Dependency]
        public IImporter[] Importers { get; set; }

        [Dependency]
        public ILoadingManager LoadingManager { get; set; }

        [Dependency]
        public IMessageDialogService MessageDialogService { get; set; }

        public IWin2DRendererProvider RendererProvider
        {
            get
            {
                return rendererProvider;
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

        public RelayCommand<ImportModel> ImportCommand
        {
            get
            {
                return importCommand ?? (importCommand = new RelayCommand<ImportModel>(x =>
                {
                    MessengerInstance.Send(new ImportMessage(x));
                }));
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

        public RelayCommand<ExportModel> ExportCommand
        {
            get
            {
                return exportCommand ?? (exportCommand = new RelayCommand<ExportModel>(async x =>
                {
                    await LoadingManager.DoWhenNotLoadingAsync(() => x.Target.ExportAsync(Document, x.Exporter, RendererProvider.Current));
                }, x => Document != null));
            }
        }

        public RelayCommand PrintCommand
        {
            get
            {
                return printCommand ?? (printCommand = new RelayCommand(async () =>
                {
                    await LoadingManager.DoWhenNotLoadingAsync(() => PrintService.PrintAsync(Document, RendererProvider.Current));
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
                }, () => Document != null));
            }
        }

        public RelayCommand SelectRightCommand
        {
            get
            {
                return selectRightCommand ?? (selectRightCommand = new RelayCommand(() =>
                {
                    Document.SelectRightOfSelectedNode();
                }, () => Document != null));
            }
        }

        public RelayCommand SelectBottomCommand
        {
            get
            {
                return selectBottomCommand ?? (selectBottomCommand = new RelayCommand(() =>
                {
                    Document.SelectBottomOfSelectedNode();
                }, () => Document != null));
            }
        }

        public RelayCommand SelectLeftCommand
        {
            get
            {
                return selectLeftCommand ?? (selectLeftCommand = new RelayCommand(() =>
                {
                    Document.SelectLeftOfSelectedNode();
                }, () => Document != null));
            }
        }

        public RelayCommand RemoveCommand
        {
            get
            {
                return removeCommand ?? (removeCommand = new RelayCommand(() =>
                {
                    Document.RemoveSelectedNodeTransactional();
                }, () => Document != null));
            }
        }

        public RelayCommand AddChildCommand
        {
            get
            {
                return addChildCommand ?? (addChildCommand = new RelayCommand(() =>
                {
                    Document.AddChildToSelectedNodeTransactional();
                }, () => Document != null));
            }
        }

        public RelayCommand AddSiblingCommand
        {
            get
            {
                return addSiblingCommand ?? (addSiblingCommand = new RelayCommand(() =>
                {
                    Document.AddSibilingToSelectedNodeTransactional();
                }, () => Document != null));
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

        public void OnDocumentChanged()
        {
            UpdateUndoRedo();

            ExportCommand.RaiseCanExecuteChanged();

            PrintCommand.RaiseCanExecuteChanged();

            AddChildCommand.RaiseCanExecuteChanged();
            AddSiblingCommand.RaiseCanExecuteChanged();

            RemoveCommand.RaiseCanExecuteChanged();

            SelectTopCommand.RaiseCanExecuteChanged();
            selectLeftCommand.RaiseCanExecuteChanged();
            SelectRightCommand.RaiseCanExecuteChanged();
            SelectBottomCommand.RaiseCanExecuteChanged();
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
