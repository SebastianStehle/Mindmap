﻿// ==========================================================================
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
        public static readonly DependencyProperty MindmapItemProperty =
            DependencyProperty.Register(nameof(MindmapItem), typeof(DocumentFileModel), typeof(RenameView), new PropertyMetadata(null));
        public DocumentFileModel MindmapItem
        {
            get { return (DocumentFileModel)GetValue(MindmapItemProperty); }
            set { SetValue(MindmapItemProperty, value); }
        }

        public RenameView()
        {
            InitializeComponent();
        }

        public override void OnOpened()
        {
            ErrorTextBlock.Opacity = 0;

            NameTextBox.Text = MindmapItem.Name;
        }

        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (!MindmapItem.CanRenameTo(NameTextBox.Text))
            {
                ErrorTextBlock.Opacity = 1;
            }
            else
            {
                try
                {
                    await MindmapItem.RenameAsync(NameTextBox.Text);
                }
                catch (FileNotFoundException)
                {
                    ErrorTextBlock.Opacity = 1;
                }

                Flyout.Hide();
            }
        }
    }
}
