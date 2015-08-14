// ==========================================================================
// AssemblyInfo.cs
// Hercules.App Application
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using Windows.UI.Xaml;

namespace Hercules.App
{
    public sealed partial class NodeListItem
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
