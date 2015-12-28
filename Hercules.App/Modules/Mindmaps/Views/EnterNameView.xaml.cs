// ==========================================================================
// EnterNameView.xaml.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System.IO;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls.Primitives;
using GP.Windows.UI;
using Hercules.App.Modules.Mindmaps.ViewModels;

namespace Hercules.App.Modules.Mindmaps.Views
{
    public sealed partial class EnterNameView : IPopupControl
    {
        public Popup Popup { get; set; }

        public EnterNameView()
        {
            InitializeComponent();
        }

        public override void OnOpened()
        {
            ErrorTextBlock.Opacity = 0;

            NameTextBox.Text = string.Empty;
        }

        private async void CreateButton_Click(object sender, RoutedEventArgs e)
        {
            MindmapsViewModel viewModel = (MindmapsViewModel)DataContext;

            if (!viewModel.IsValidMindmapName(NameTextBox.Text))
            {
                ErrorTextBlock.Opacity = 1;
            }
            else
            {
                try
                {
                    await viewModel.CreateNewMindmapAsync(NameTextBox.Text);
                }
                catch (FileNotFoundException)
                {
                    ErrorTextBlock.Opacity = 1;
                }

                if (Flyout != null)
                {
                    Flyout.Hide();
                }
            }
        }
    }
}
