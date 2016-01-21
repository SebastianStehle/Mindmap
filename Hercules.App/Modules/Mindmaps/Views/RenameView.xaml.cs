// ==========================================================================
// RenameView.xaml.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System.IO;
using Windows.UI.Xaml;
using Hercules.App.Components;

namespace Hercules.App.Modules.Mindmaps.Views
{
    public sealed partial class RenameView
    {
        public static readonly DependencyProperty DocumentFileProperty =
            DependencyProperty.Register(nameof(DocumentFile), typeof(DocumentFileModel), typeof(RenameView), new PropertyMetadata(null));
        public DocumentFileModel DocumentFile
        {
            get { return (DocumentFileModel)GetValue(DocumentFileProperty); }
            set { SetValue(DocumentFileProperty, value); }
        }

        public RenameView()
        {
            InitializeComponent();
        }

        public override void OnOpened()
        {
            ErrorTextBlock.Opacity = 0;

            NameTextBox.Text = DocumentFile.Name;
        }

        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (!DocumentFile.CanRenameTo(NameTextBox.Text))
            {
                ErrorTextBlock.Opacity = 1;
            }
            else
            {
                try
                {
                    await DocumentFile.RenameAsync(NameTextBox.Text);
                }
                catch (FileNotFoundException)
                {
                    ErrorTextBlock.Opacity = 1;
                }

                Flyout?.Hide();
            }
        }
    }
}
