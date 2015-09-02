// ==========================================================================
// EnterNameView.xaml.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls.Primitives;
using GP.Windows.UI;

namespace Hercules.App.Modules.Mindmaps.Views
{
    public sealed partial class RenameView : IPopupControl
    {
        public static readonly DependencyProperty MindmapItemProperty =
            DependencyProperty.Register("MindmapItem", typeof(MindmapItem), typeof(RenameView), new PropertyMetadata(null));
        public MindmapItem MindmapItem
        {
            get { return (MindmapItem)GetValue(MindmapItemProperty); }
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
                await MindmapItem.RenameAsync(TitleTextBox.Text);

                Flyout.Hide();
            }
        }
    }
}
