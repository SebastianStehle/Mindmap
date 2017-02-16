// ==========================================================================
// EditorPropertiesViewModel.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Windows.UI;
using GalaSoft.MvvmLight.Command;
using GP.Utils;
using GP.Utils.Mvvm;
using GP.Utils.UI;
using Hercules.App.Components;
using Hercules.Model;
using Hercules.Win2D.Rendering;
using Microsoft.Practices.Unity;
using PropertyChanged;

// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable InvertIf

namespace Hercules.App.Modules.Editor.ViewModels
{
    [ImplementPropertyChanged]
    public sealed class EditorPropertiesViewModel : DocumentViewModelBase
    {
        private static readonly string[] ImageExtensions = { ".png", ".jpg", ".jpeg" };
        private readonly ObservableCollection<INodeColor> customColors = new ObservableCollection<INodeColor>();
        private readonly ObservableCollection<INodeColor> themeColors = new ObservableCollection<INodeColor>();
        private readonly ObservableCollection<INodeIcon> customIcons = new ObservableCollection<INodeIcon>();
        private RelayCommand<Color> addColorCommand;
        private RelayCommand<INodeColor> changeColorCommand;
        private RelayCommand<INodeIcon> changeIconCommand;
        private RelayCommand<int> changeIconPositionCommand;
        private RelayCommand<int> changeIconSizeCommand;
        private RelayCommand<int> changeShapeCommand;
        private RelayCommand<int> changeCheckableModeCommand;
        private RelayCommand toggleNotesCommand;
        private RelayCommand toggleIsCheckableCommand;
        private RelayCommand toggleHullCommand;
        private RelayCommand addIconCommand;

        [Dependency]
        public IProcessManager ProcessManager { get; set; }

        [Dependency]
        public IDialogService MessageDialogService { get; set; }

        [Dependency]
        public ISettingsProvider SettingsProvider { get; set; }

        public ObservableCollection<INodeColor> CustomColors
        {
            get { return customColors; }
        }

        public ObservableCollection<INodeColor> ThemeColors
        {
            get { return themeColors; }
        }

        public ObservableCollection<INodeIcon> CustomIcons
        {
            get { return customIcons; }
        }

        public bool ShowIconSection
        {
            get
            {
                return SettingsProvider?.ShowIconSection == true;
            }
            set
            {
                if (!Equals(SettingsProvider.ShowIconSection, value))
                {
                    SettingsProvider.ShowIconSection = value;
                    RaisePropertyChanged(nameof(ShowIconSection));
                }
            }
        }

        public bool ShowColorSection
        {
            get
            {
                return SettingsProvider?.ShowColorSection == true;
            }
            set
            {
                if (!Equals(SettingsProvider.ShowColorSection, value))
                {
                    SettingsProvider.ShowColorSection = value;
                    RaisePropertyChanged(nameof(ShowColorSection));
                }
            }
        }

        public bool ShowNotesSection
        {
            get
            {
                return SettingsProvider?.ShowNotesSection == true;
            }
            set
            {
                if (!Equals(SettingsProvider.ShowNotesSection, value))
                {
                    SettingsProvider.ShowNotesSection = value;
                    RaisePropertyChanged(nameof(ShowNotesSection));
                }
            }
        }

        public bool ShowShapeSection
        {
            get
            {
                return SettingsProvider?.ShowShapeSection == true;
            }
            set
            {
                if (!Equals(SettingsProvider.ShowShapeSection, value))
                {
                    SettingsProvider.ShowShapeSection = value;
                    RaisePropertyChanged(nameof(ShowShapeSection));
                }
            }
        }

        public bool ShowCheckBoxesSection
        {
            get
            {
                return SettingsProvider?.ShowCheckBoxesSection == true;
            }
            set
            {
                if (!Equals(SettingsProvider.ShowCheckBoxesSection, value))
                {
                    SettingsProvider.ShowCheckBoxesSection = value;
                    RaisePropertyChanged(nameof(ShowCheckBoxesSection));
                }
            }
        }

        public ICommand ToggleNotesCommand
        {
            get
            {
                return toggleNotesCommand ?? (toggleNotesCommand = new RelayCommand(() =>
                {
                    SelectedNode.ToggleNotesTransactional();
                },
                () => Document != null && SelectedNode != null).DependentOn(this, nameof(Document), nameof(SelectedNode)));
            }
        }

        public ICommand AddIconCommand
        {
            get
            {
                return addIconCommand ?? (addIconCommand = new RelayCommand(async () =>
                {
                    await MessageDialogService.OpenFileDialogAsync(ImageExtensions, async (name, stream) =>
                    {
                        var attachmentIcon = await AttachmentIcon.TryCreateAsync(name, await stream.ToMemoryStreamAsync());

                        if (attachmentIcon != null)
                        {
                            if (!customIcons.Contains(attachmentIcon))
                            {
                                customIcons.Add(attachmentIcon);
                            }
                        }
                        else
                        {
                            MessageDialogService.AlertLocalizedAsync("Properties_LoadingIconFailed_Alert").Forget();
                        }
                    });
                },
                () => Document != null)).DependentOn(this, nameof(Document));
            }
        }

        public ICommand AddColorCommand
        {
            get
            {
                return addColorCommand ?? (addColorCommand = new RelayCommand<Color>(x =>
                {
                    var value = new ValueColor(ColorsHelper.ConvertToInt(x));

                    if (!customColors.Contains(value))
                    {
                        customColors.Add(value);
                    }
                },
                x => Document != null)).DependentOn(this, nameof(Document));
            }
        }

        public ICommand ChangeShapeCommand
        {
            get
            {
                return changeShapeCommand ?? (changeShapeCommand = new RelayCommand<int>(x =>
                {
                    var node = (Node)SelectedNode;

                    if (x == 0)
                    {
                        node.ChangeShapeTransactional(null);
                    }
                    else
                    {
                        node.ChangeShapeTransactional((NodeShape)(x - 1));
                    }
                },
                x => SelectedNode is Node)).DependentOn(this, nameof(SelectedNode));
            }
        }

        public ICommand ChangeIconCommand
        {
            get
            {
                return changeIconCommand ?? (changeIconCommand = new RelayCommand<INodeIcon>(x =>
                {
                    SelectedNode.ChangeIconTransactional(x);
                },
                x => SelectedNode != null).DependentOn(this, nameof(SelectedNode)));
            }
        }

        public ICommand ChangeIconSizeCommand
        {
            get
            {
                return changeIconSizeCommand ?? (changeIconSizeCommand = new RelayCommand<int>(x =>
                {
                    SelectedNode.ChangeIconSizeTransactional((IconSize)x);
                },
                x => SelectedNode != null).DependentOn(this, nameof(SelectedNode)));
            }
        }

        public ICommand ChangeCheckableModeCommand
        {
            get
            {
                return changeCheckableModeCommand ?? (changeCheckableModeCommand = new RelayCommand<int>(x =>
                {
                    SelectedNode.ChangeCheckableModeTransactional((CheckableMode)x);
                },
                x => SelectedNode != null).DependentOn(this, nameof(SelectedNode)));
            }
        }

        public ICommand ChangeIconPositionCommand
        {
            get
            {
                return changeIconPositionCommand ?? (changeIconPositionCommand = new RelayCommand<int>(x =>
                {
                    SelectedNode.ChangeIconPositionTransactional((IconPosition)x);
                },
                x => SelectedNode != null).DependentOn(this, nameof(SelectedNode)));
            }
        }

        public ICommand ChangeColorCommand
        {
            get
            {
                return changeColorCommand ?? (changeColorCommand = new RelayCommand<INodeColor>(x =>
                {
                    SelectedNode.ChangeColorTransactional(x);
                },
                x => SelectedNode != null).DependentOn(this, nameof(SelectedNode)));
            }
        }

        public ICommand ToggleHullCommand
        {
            get
            {
                return toggleHullCommand ?? (toggleHullCommand = new RelayCommand(() =>
                {
                    SelectedNode.ToggleHullTransactional();
                },
                () => SelectedNode != null).DependentOn(this, nameof(SelectedNode)));
            }
        }

        public ICommand ToggleIsCheckableCommand
        {
            get
            {
                return toggleIsCheckableCommand ?? (toggleIsCheckableCommand = new RelayCommand(() =>
                {
                    Document.ToggleCheckableTransactional();
                },
                () => Document != null).DependentOn(this, nameof(Document)));
            }
        }

        public EditorPropertiesViewModel(IMindmapStore mindmapStore, IWin2DRendererProvider rendererProvider)
            : base(mindmapStore, rendererProvider)
        {
            rendererProvider.RendererCreated += RendererProvider_RendererCreated;
        }

        protected override void OnDocumentChanged(Document oldDocument, Document newDocument)
        {
            customColors.Clear();
            customIcons.Clear();

            if (newDocument == null)
            {
                return;
            }

            var recentColors =
                newDocument.UndoRedoManager.Commands()
                    .OfType<ChangeColorCommand>().Select(x => x.NewColor)
                    .OfType<ValueColor>()
                    .Distinct();

            foreach (var recent in recentColors)
            {
                customColors.Add(recent);
            }

            var recentIcons =
                newDocument.UndoRedoManager.Commands()
                    .OfType<ChangeIconCommand>().Select(x => x.NewIcon)
                    .OfType<AttachmentIcon>()
                    .Distinct();

            foreach (var recent in recentIcons)
            {
                customIcons.Add(recent);
            }
        }

        private void RendererProvider_RendererCreated(object sender, EventArgs e)
        {
            themeColors.Clear();

            for (var i = 0; i < RendererProvider.Current.Resources.Colors.Count; i++)
            {
                themeColors.Add(new ThemeColor(i));
            }
        }
    }
}