// ==========================================================================
// EnterNameView.xaml.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================
using Hercules.App.ViewModels;
using GP.Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls.Primitives;

namespace Hercules.App
{
    public partial class EnterNameView : IPopupControl
    {
        public Popup Popup { get; set; }

        public EnterNameView()
        {
            InitializeComponent();
        }

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
    }
}
