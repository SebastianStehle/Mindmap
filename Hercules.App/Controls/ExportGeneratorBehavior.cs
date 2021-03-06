﻿// ==========================================================================
// ExportGeneratorBehavior.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using GP.Utils;
using GP.Utils.UI;
using GP.Utils.UI.Interactivity;
using Hercules.App.Modules;
using Hercules.App.Modules.Editor.ViewModels;

namespace Hercules.App.Controls
{
    public sealed class ExportGeneratorBehavior : Behavior<MenuFlyoutSubItem>
    {
        public static readonly DependencyProperty EditorViewModelProperty =
            DependencyPropertyManager.Register<ExportGeneratorBehavior, EditorViewModel>(nameof(EditorViewModel), null, e => e.Owner.BindItems());
        public EditorViewModel EditorViewModel
        {
            get { return (EditorViewModel)GetValue(EditorViewModelProperty); }
            set { SetValue(EditorViewModelProperty, value); }
        }

        protected override void OnAttached()
        {
            BindItems();
        }

        private void BindItems()
        {
            var viewModel = EditorViewModel;

            if (viewModel == null)
            {
                return;
            }

            var items = AssociatedElement.Items;

            if (items == null)
            {
                return;
            }

            foreach (var target in viewModel.ExportTargets)
            {
                var text = LocalizationManager.GetString($"ExportTarget_{target.NameKey}");

                var targetItem = new MenuFlyoutSubItem { Text = text };

                foreach (var exporter in viewModel.Exporters)
                {
                    text = LocalizationManager.GetString($"Exporter_{exporter.NameKey}");

                    var viewModelParameter = new ExportModel { Target = target, Exporter = exporter };

                    var exportButton =
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
