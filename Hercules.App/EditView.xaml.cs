// ==========================================================================
// EditView.cs
// RavenMind Application
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using GalaSoft.MvvmLight.Messaging;
using Hercules.App.Controls;
using Hercules.App.Messages;
using System;
using Windows.ApplicationModel.Resources;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Hercules.App
{
    public sealed partial class EditView : MindmapFlyoutView
    {
        private string oldName;

        public EditView()
        {
            InitializeComponent();
        }

        public override void OnOpened()
        {
            NameTextBox.Name = oldName = Document.Name;
        }

        public override void OnClosed()
        {
            if (oldName != Document.Name)
            {
                Messenger.Default.Send(new NameChangedMessage(Document.Name));
            }
        }

        private void NameTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(NameTextBox.Text))
            {
                ErrorTextBlock.Visibility = Visibility.Collapsed;
            }
            else
            {
                ErrorTextBlock.Visibility = Visibility.Visible;
            }
        }

        private async void DeleteMindmapButton_Click(object sender, RoutedEventArgs e)
        {
            ResourceLoader resourceLoader = new ResourceLoader();

            MessageDialog messageDialog = new MessageDialog(resourceLoader.GetString("DeleteMindmapText"), resourceLoader.GetString("DeleteMindmapTitle"));

            messageDialog.Commands.Add(new UICommand(resourceLoader.GetString("Yes"), null, 0));
            messageDialog.Commands.Add(new UICommand(resourceLoader.GetString("No"), null, 1));
            messageDialog.DefaultCommandIndex = 1;

            IUICommand commandChosen = await messageDialog.ShowAsync();

            if (commandChosen == messageDialog.Commands[0])
            {
                Messenger.Default.Send(new DeleteMindmapMessage(Document.Id));
            }
        }
    }
}
