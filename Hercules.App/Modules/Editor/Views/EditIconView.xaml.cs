// ==========================================================================
// EditIconView.xaml.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System;
using System.IO;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;
using GP.Windows;
using GP.Windows.Mvvm;
using Hercules.Model;
using Hercules.Model.Utils;
using Microsoft.Practices.Unity;

namespace Hercules.App.Modules.Editor.Views
{
    public sealed partial class EditIconView
    {
        private bool hasChange;

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
                if (selectedNode.Icon == null)
                {
                    IconsGrid.SelectedIndex = -1;
                }
                else
                {
                    KeyIcon keyIcon = selectedNode.Icon as KeyIcon;

                    if (keyIcon != null)
                    {
                        IconsGrid.SelectedItem = keyIcon.Key;
                    }
                }
            }

            hasChange = false;
        }

        private void IconsGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string selected = IconsGrid.SelectedItem as string;

            Change(selected != null ? new KeyIcon(selected) : null);
        }

        private void RemoveIconButton_Click(object sender, RoutedEventArgs e)
        {
            Change(null);
        }

        private void Change(INodeIcon selected)
        {
            NodeBase selectedNode = Document?.SelectedNode;

            if (selectedNode != null)
            {
                if (!Equals(selected, selectedNode.Icon))
                {
                    if (hasChange && Document.UndoRedoManager.IsLastCommand<ChangeIconCommand>(x => x.Node.Id == selectedNode.Id))
                    {
                        Document.UndoRedoManager.Revert();
                    }

                    if (!Equals(selected, selectedNode.Icon))
                    {
                        selectedNode.ChangeIconKeyTransactional(selected);

                        hasChange = true;
                    }
                }
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
                        if (await AttachmentIcon.ValidateAsync(fileStream))
                        {
                            AttachmentIcon attachmentIcon = new AttachmentIcon(fileStream.AsStreamForRead(), file.DisplayName);

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
