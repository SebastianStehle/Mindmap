// ==========================================================================
// EditorViewModel.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System;
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
using Microsoft.Practices.Unity;
using PropertyChanged;

namespace Hercules.App.Modules.Editor.ViewModels
{
    [ImplementPropertyChanged]
    public sealed class EditorViewModel : DocumentViewModelBase
    {
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
        public IImportSource[] ImportSources { get; set; }

        [Dependency]
        public IImporter[] Importers { get; set; }

        [Dependency]
        public IProcessManager ProcessManager { get; set; }

        [Dependency]
        public IMessageDialogService MessageDialogService { get; set; }

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
                },
                () => Document != null && Document.UndoRedoManager.CanRedo).DependentOn(this, nameof(Document)));
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
                },
                () => Document != null && Document.UndoRedoManager.CanUndo).DependentOn(this, nameof(Document)));
            }
        }

        public RelayCommand<ExportModel> ExportCommand
        {
            get
            {
                return exportCommand ?? (exportCommand = new RelayCommand<ExportModel>(async x =>
                {
                    await ProcessManager.RunMainProcessAsync(this, () => x.Target.ExportAsync(MindmapStore.LoadedMindmap.Name, Document, x.Exporter, RendererProvider.Current));
                },
                x => Document != null).DependentOn(this, nameof(Document)));
            }
        }

        public RelayCommand PrintCommand
        {
            get
            {
                return printCommand ?? (printCommand = new RelayCommand(async () =>
                {
                    await ProcessManager.RunMainProcessAsync(this, () => PrintService.PrintAsync(Document, RendererProvider.Current));
                },
                () => Document != null).DependentOn(this, nameof(Document)));
            }
        }

        public RelayCommand SelectTopCommand
        {
            get
            {
                return selectTopCommand ?? (selectTopCommand = new RelayCommand(() =>
                {
                    Document.SelectTopOfSelectedNode();
                },
                () => Document != null && SelectedNode != null).DependentOn(this, nameof(Document), nameof(SelectedNode)));
            }
        }

        public RelayCommand SelectRightCommand
        {
            get
            {
                return selectRightCommand ?? (selectRightCommand = new RelayCommand(() =>
                {
                    Document.SelectRightOfSelectedNode();
                },
                () => Document != null && SelectedNode != null).DependentOn(this, nameof(Document), nameof(SelectedNode)));
            }
        }

        public RelayCommand SelectBottomCommand
        {
            get
            {
                return selectBottomCommand ?? (selectBottomCommand = new RelayCommand(() =>
                {
                    Document.SelectBottomOfSelectedNode();
                },
                () => Document != null && SelectedNode != null).DependentOn(this, nameof(Document), nameof(SelectedNode)));
            }
        }

        public RelayCommand SelectLeftCommand
        {
            get
            {
                return selectLeftCommand ?? (selectLeftCommand = new RelayCommand(() =>
                {
                    Document.SelectLeftOfSelectedNode();
                },
                () => Document != null && SelectedNode != null).DependentOn(this, nameof(Document), nameof(SelectedNode)));
            }
        }

        public RelayCommand RemoveCommand
        {
            get
            {
                return removeCommand ?? (removeCommand = new RelayCommand(() =>
                {
                    Document.RemoveSelectedNodeTransactional();
                },
                () => Document != null && SelectedNode is Node).DependentOn(this, nameof(Document), nameof(SelectedNode)));
            }
        }

        public RelayCommand AddChildCommand
        {
            get
            {
                return addChildCommand ?? (addChildCommand = new RelayCommand(() =>
                {
                    Document.AddChildToSelectedNodeTransactional();
                },
                () => Document != null && SelectedNode != null).DependentOn(this, nameof(Document), nameof(SelectedNode)));
            }
        }

        public RelayCommand AddSiblingCommand
        {
            get
            {
                return addSiblingCommand ?? (addSiblingCommand = new RelayCommand(() =>
                {
                    Document.AddSibilingToSelectedNodeTransactional();
                },
                () => Document != null && SelectedNode is Node).DependentOn(this, nameof(Document), nameof(SelectedNode)));
            }
        }

        public EditorViewModel()
        {
            Messenger.Default.Register<SaveMindmapMessage>(this, OnSaveMindmap);
        }

        public EditorViewModel(IMindmapStore mindmapStore, IWin2DRendererProvider rendererProvider)
            : base(mindmapStore, rendererProvider)
        {
            Messenger.Default.Register<SaveMindmapMessage>(this, OnSaveMindmap);
        }

        protected override void OnDocumentChanged(Document oldDocument, Document newDocument)
        {
            if (oldDocument != null)
            {
                oldDocument.StateChanged -= UndoRedoManager_StateChanged;
            }

            if (newDocument != null)
            {
                newDocument.StateChanged += UndoRedoManager_StateChanged;
            }

            UpdateUndoRedo();
        }

        private async void OnSaveMindmap(SaveMindmapMessage message)
        {
            await MindmapStore.SaveAsync();

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
