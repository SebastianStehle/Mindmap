// ==========================================================================
// AssemblyInfo.cs
// MindmapApp Application
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace MindmapApp
{
    public sealed partial class NodeListItem : UserControl
    {
        private bool isCollapsed;

        public NodeListItem()
        {
            this.InitializeComponent();
        }

        private void CollapseButton_Click(object sender, RoutedEventArgs e)
        {
            if (isCollapsed)
            {
                VisualStateManager.GoToState(this, "Normal", true);
            }
            else
            {
                VisualStateManager.GoToState(this, "Collapsed", true);
            }

            isCollapsed = !isCollapsed;
        }
    }
}
