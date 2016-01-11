// ==========================================================================
// HelpView.xaml.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using Windows.ApplicationModel;
using Windows.UI.Xaml;
using GP.Utils.UI;

namespace Hercules.App.Modules.Editor.Views
{
    public sealed partial class HelpView
    {
        public HelpView()
        {
            InitializeComponent();

            if (!DesignMode.DesignModeEnabled)
            {
                HelpTextControl.Style =
                    VisualTreeExtensions.LoadFromAppResource<Style>("HelpText_{culture}") ??
                    VisualTreeExtensions.LoadFromAppResource<Style>("HelpText_en-US");
            }
        }
    }
}
