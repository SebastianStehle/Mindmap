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
using Hercules.App.Components;

namespace Hercules.App.Modules.Mindmaps.Views
{
    public sealed partial class RenameView : IPopupControl
    {
        public static readonly DependencyProperty MindmapItemProperty =
            DependencyProperty.Register("MindmapItem", typeof(IMindmapRef), typeof(RenameView), new PropertyMetadata(null));
        public IMindmapRef MindmapItem
        {
            get { return (IMindmapRef)GetValue(MindmapItemProperty); }
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

            TitleTextBox.Text = MindmapItem.Title;
        }

        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TitleTextBox.Text))
            {
                ErrorTextBlock.Opacity = 1;
            }
            else
            {
                try
                {
                    await MindmapItem.RenameAsync(TitleTextBox.Text);
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
