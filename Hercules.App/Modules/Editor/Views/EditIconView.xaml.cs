// ==========================================================================
// EditIconView.xaml.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System;
using System.Collections.Generic;
using System.Linq;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using GP.Windows;
using GP.Windows.Mvvm;
using Hercules.Model;
using Hercules.Model.Utils;
using Microsoft.Practices.Unity;

namespace Hercules.App.Modules.Editor.Views
{
    public sealed partial class EditIconView
    {
        public EditIconView()
        {
            InitializeComponent();

            IconsGrid.SelectionChanged += IconsGrid_SelectionChanged;
        }

        public override void OnOpened()
        {
            NodeBase selectedNode = Document?.SelectedNode;

            if (selectedNode != null)
            {
                HashSet<AttachmentIcon> recentIcons = new HashSet<AttachmentIcon>(
                    selectedNode.Document.UndoRedoManager.Commands()
                        .OfType<ChangeIconCommand>().Select(x => x.NewIcon)
                        .OfType<AttachmentIcon>()
                        .Distinct()
                        .Take(6));

                IconsRecentGrid.ItemsSource = recentIcons;

                KeyIcon keyIcon = selectedNode.Icon as KeyIcon;

                if (keyIcon != null)
                {
                    IconsGrid.SelectedItem = keyIcon.Key;
                }
                else
                {
                    ResetIconSelection();
                }

                AttachmentIcon attachmentIcon = selectedNode.Icon as AttachmentIcon;

                if (attachmentIcon != null)
                {
                    IconsRecentGrid.SelectedIndex = 0;
                }
                else
                {
                    ResetRecentSelection();
                }

                IconSizeListBox.SelectedIndex = (int)selectedNode.IconSize;

                IconPositionListBox.SelectedIndex = (int)selectedNode.IconPosition;
            }
        }

        private void IconPositionListBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Change((IconPosition)IconPositionListBox.SelectedIndex);
        }

        private void IconSizeListBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Change((IconSize)IconSizeListBox.SelectedIndex);
        }

        private void IconsRecentGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            AttachmentIcon selected = IconsRecentGrid.SelectedItem as AttachmentIcon;

            if (selected != null)
            {
                ResetIconSelection();

                Change(selected);
            }
        }

        private void IconsGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string selected = IconsGrid.SelectedItem as string;

            if (selected != null)
            {
                ResetRecentSelection();

                Change(new KeyIcon(selected));
            }
        }

        private void RemoveIconButton_Click(object sender, RoutedEventArgs e)
        {
            ResetRecentSelection();
            ResetIconSelection();

            Change(null);
        }

        private void ResetRecentSelection()
        {
            IconsRecentGrid.SelectedIndex = -1;
        }

        private void ResetIconSelection()
        {
            IconsGrid.SelectedIndex = -1;
        }

        private void Change(IconSize selected)
        {
            NodeBase selectedNode = Document?.SelectedNode;

            if (selectedNode != null)
            {
                selectedNode.ChangeIconSizeTransactional(selected);
            }
        }

        private void Change(IconPosition selected)
        {
            NodeBase selectedNode = Document?.SelectedNode;

            if (selectedNode != null)
            {
                selectedNode.ChangeIconPositionTransactional(selected);
            }
        }

        private void Change(INodeIcon selected)
        {
            NodeBase selectedNode = Document?.SelectedNode;

            if (selectedNode != null)
            {
                selectedNode.ChangeIconKeyTransactional(selected);
            }
        }

        private async void OpenFileButton_Click(object sender, RoutedEventArgs e)
        {
            IProcessManager progressManager = ViewModelLocator.Container.Resolve<IProcessManager>();

            FileOpenPicker filePicker = new FileOpenPicker();

            filePicker.FileTypeFilter.Add(".png");
            filePicker.FileTypeFilter.Add(".jpg");
            filePicker.FileTypeFilter.Add(".jpeg");

            StorageFile file = await filePicker.PickSingleFileAsync();

            if (file != null)
            {
                progressManager.RunMainProcessAsync(this, async () =>
                {
                    using (IRandomAccessStream fileStream = await file.OpenAsync(FileAccessMode.ReadWrite))
                    {
                        AttachmentIcon attachmentIcon = await AttachmentIcon.TryCreateAsync(file.DisplayName, fileStream);

                        if (attachmentIcon != null)
                        {
                            Change(attachmentIcon);
                        }
                        else
                        {
                            string content = ResourceManager.GetString("LoadingIconFailed_Content");
                            string heading = ResourceManager.GetString("LoadingIconFailed_Heading");

                            await ViewModelLocator.Container.Resolve<IMessageDialogService>().AlertAsync(content, heading);
                        }
                    }
                }).Forget();
            }
        }
    }
}
