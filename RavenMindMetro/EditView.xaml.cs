// ==========================================================================
// EditView.cs
// RavenMind Application
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using GalaSoft.MvvmLight.Messaging;
using RavenMind.Controls;
using RavenMind.Messages;
using System;
using System.Threading.Tasks;
using Windows.ApplicationModel.Resources;
using Windows.Foundation;
using Windows.UI.Popups;
using Windows.UI.StartScreen;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace RavenMind
{
    public sealed partial class EditView : UserControl
    {
        #region Fields

        private string oldName;

        #endregion

        #region Properties

        public Mindmap Mindmap { get; set; }

        #endregion

        #region Constructors

        public EditView()
        {
            InitializeComponent();

            Loaded += EditView_Loaded;
        }

        public EditView(Mindmap mindmap)
            : this()
        {
            Mindmap = mindmap;

            DataContext = mindmap.Document;
        }

        #endregion

        #region Methods

        private void EditView_Loaded(object sender, RoutedEventArgs e)
        {
            NameTextBox.Name = oldName = Mindmap.Name;

            string idString = Mindmap.Document.Id.ToString();

            if (SecondaryTile.Exists(idString))
            {
                UnpinMindmapButton.Visibility = Visibility.Visible;

                PinMindmapButton.Visibility = Visibility.Collapsed;
            }
        }

        private void EditView_Unloaded(object sender, RoutedEventArgs e)
        {
            if (oldName != Mindmap.Name)
            {
                Messenger.Default.Send(new NameChangedMessage(Mindmap.Name));
            }
        }

        private void NameTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (Mindmap != null)
            {
                if (!string.IsNullOrWhiteSpace(NameTextBox.Text))
                {
                    Mindmap.Name = NameTextBox.Text;

                    ErrorTextBlock.Visibility = Visibility.Collapsed;
                }
                else
                {
                    ErrorTextBlock.Visibility = Visibility.Visible;
                }
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
                await Unpin();

                Messenger.Default.Send(new DeleteMindmapMessage(Mindmap.Document.Id));
            }
        }

        private async void UnpinMindmapButton_Click(object sender, RoutedEventArgs e)
        {
            await Unpin();
        }

        private async void PinMindmapButton_Click(object sender, RoutedEventArgs e)
        {
            string idString = Mindmap.Document.Id.ToString();

            if (!SecondaryTile.Exists(idString))
            {
                Uri logo  = new Uri("ms-appx:///Assets/Logo.png");

                SecondaryTile tile = new SecondaryTile(idString, Mindmap.Document.Name, Mindmap.Document.Name, idString, TileOptions.ShowNameOnLogo | TileOptions.ShowNameOnWideLogo, logo);

                PinMindmapButton.IsEnabled = false;

                bool isPinned = await tile.RequestCreateForSelectionAsync(CalculatePinButtonRect());

                if (isPinned)
                {
                    PinMindmapButton.IsEnabled = true;
                    PinMindmapButton.Visibility = Visibility.Collapsed;

                    UnpinMindmapButton.Visibility = Visibility.Visible;
                }
            }
        }

        private async Task Unpin()
        {
            string idString = Mindmap.Document.Id.ToString();

            if (SecondaryTile.Exists(idString))
            {
                SecondaryTile tile = new SecondaryTile(idString);

                UnpinMindmapButton.IsEnabled = false;

                await tile.RequestDeleteAsync();

                UnpinMindmapButton.IsEnabled = true;
                UnpinMindmapButton.Visibility = Visibility.Collapsed;

                PinMindmapButton.Visibility = Visibility.Visible;
            }
        }

        private Rect CalculatePinButtonRect()
        {
            GeneralTransform buttonTransform = PinMindmapButton.TransformToVisual(null);

            return buttonTransform.TransformBounds(new Rect(new Point(0, 0), PinMindmapButton.RenderSize));
        }

        #endregion
    }
}
