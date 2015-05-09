// ==========================================================================
// MindmapsViewModel.cs
// RavenMind Application
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using RavenMind.ViewModels;
using SE.Metro.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;

namespace RavenMind
{
    public partial class EnterNameView : UserControl, IPopupControl
    {
        #region Properties

        public Popup Popup { get; set; }

        #endregion
        
        #region Constructors

        public EnterNameView()
        {
            InitializeComponent();
        }

        #endregion

        #region Methods

        private async void CreateButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(NameTextBox.Text))
            {
                ErrorTextBlock.Visibility = Visibility.Visible;
            }
            else
            {
                MindmapsViewModel viewModel = (MindmapsViewModel)DataContext;

                await viewModel.CreateNewMindmapAsync(NameTextBox.Text, NameTextBox.Text);

                Popup.IsOpen = false;
            }
        }

        #endregion
    }
}
