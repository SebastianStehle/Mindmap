// ==========================================================================
// ImportGeneratorBehavior.cs
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
    public sealed class ImportGeneratorBehavior : Behavior<MenuFlyout>
    {
        public static readonly DependencyProperty EditorViewModelProperty =
            DependencyPropertyManager.Register<ImportGeneratorBehavior, EditorViewModel>(nameof(EditorViewModel), null, e => e.Owner.BindItems());
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

            foreach (var source in viewModel.ImportSources)
            {
                var text = LocalizationManager.GetString($"ImportSource_{source.NameKey}");

                var targetItem = new MenuFlyoutSubItem { Text = text };

                foreach (var importer in viewModel.Importers)
                {
                    text = LocalizationManager.GetString($"Importer_{importer.NameKey}");

                    var viewModelParameter = new ImportModel { Source = source, Importer = importer };

                    var importButton =
                        VisualTreeExtensions.CreateMenuItem(text,
                            viewModel.ImportCommand,
                            viewModelParameter);

                    targetItem.Items?.Add(importButton);
                }

                items.Add(targetItem);
            }
        }
    }
}
