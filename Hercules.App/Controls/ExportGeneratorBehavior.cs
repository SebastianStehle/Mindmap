// ==========================================================================
// ExportGeneratorBehavior.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using GP.Windows.UI;
using GP.Windows.UI.Interactivity;
using Hercules.App.Modules;
using Hercules.App.Modules.Editor.ViewModels;
using Hercules.Model.ExImport;
using Hercules.Model.Utils;

namespace Hercules.App.Controls
{
    public sealed class ExportGeneratorBehavior : Behavior<MenuFlyoutSubItem>
    {
        public static readonly DependencyProperty EditorViewModelProperty =
            DependencyProperty.Register("EditorViewModel", typeof(EditorViewModel), typeof(ExportGeneratorBehavior), new PropertyMetadata(null, (d, e) => ((ExportGeneratorBehavior)d).OnEditorViewModelChanged()));
        public EditorViewModel EditorViewModel
        {
            get { return (EditorViewModel)GetValue(EditorViewModelProperty); }
            set { SetValue(EditorViewModelProperty, value); }
        }

        private void OnEditorViewModelChanged()
        {
            BindItems();
        }

        protected override void OnAttached()
        {
            BindItems();
        }

        private void BindItems()
        {
            EditorViewModel viewModel = EditorViewModel;

            if (viewModel != null)
            {
                var items = AssociatedElement.Items;

                if (items != null)
                {
                    foreach (IExportTarget target in viewModel.ExportTargets)
                    {
                        string text = ResourceManager.GetString($"ExportTarget_{target.NameKey}");

                        MenuFlyoutSubItem targetItem = new MenuFlyoutSubItem { Text = text };

                        foreach (IExporter exporter in viewModel.Exporters)
                        {
                            text = ResourceManager.GetString($"Exporter_{exporter.NameKey}");

                            var viewModelParameter = new ExportModel { Target = target, Exporter = exporter };

                            MenuFlyoutItem exportButton =
                                VisualTreeExtensions.CreateMenuItem(text,
                                    viewModel.ExportCommand,
                                    viewModelParameter);

                            targetItem.Items?.Add(exportButton);
                        }

                        items.Add(targetItem);
                    }
                }
            }
        }
    }
}
