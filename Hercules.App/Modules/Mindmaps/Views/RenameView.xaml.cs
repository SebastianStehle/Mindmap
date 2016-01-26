// ==========================================================================
// RenameView.xaml.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System.IO;
using Windows.UI.Xaml;
using GP.Utils;
using Hercules.App.Components;

namespace Hercules.App.Modules.Mindmaps.Views
{
    public sealed partial class RenameView
    {
        public static readonly DependencyProperty MindmapStoreProperty =
            DependencyProperty.Register(nameof(MindmapStore), typeof(IMindmapStore), typeof(RenameView), new PropertyMetadata(null));
        public IMindmapStore MindmapStore
        {
            get { return (IMindmapStore)GetValue(MindmapStoreProperty); }
            set { SetValue(MindmapStoreProperty, value); }
        }

        public static readonly DependencyProperty DocumentFileProperty =
            DependencyProperty.Register(nameof(DocumentFile), typeof(IDocumentFileModel), typeof(RenameView), new PropertyMetadata(null));
        public IDocumentFileModel DocumentFile
        {
            get { return (IDocumentFileModel)GetValue(DocumentFileProperty); }
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
            if (!NameTextBox.Text.IsValidFileName())
            {
                ErrorTextBlock.Opacity = 1;
            }
            else
            {
                try
                {
                    await MindmapStore.RenameAsync(DocumentFile, NameTextBox.Text);
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
