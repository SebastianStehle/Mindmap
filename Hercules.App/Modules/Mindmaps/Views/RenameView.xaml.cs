// ==========================================================================
// RenameView.xaml.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System.IO;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls.Primitives;
using GP.Windows.UI;
using Hercules.App.Components;

namespace Hercules.App.Modules.Mindmaps.Views
{
    public sealed partial class RenameView : IPopupControl
    {
        public static readonly DependencyProperty MindmapItemProperty =
            DependencyProperty.Register("MindmapItem", typeof(MindmapRef), typeof(RenameView), new PropertyMetadata(null));
        public MindmapRef MindmapItem
        {
            get { return (MindmapRef)GetValue(MindmapItemProperty); }
            set { SetValue(MindmapItemProperty, value); }
        }

        public Popup Popup { get; set; }

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
            if (string.IsNullOrWhiteSpace(NameTextBox.Text))
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
