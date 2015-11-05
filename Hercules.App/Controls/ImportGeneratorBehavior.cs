// ==========================================================================
// ImportGeneratorBehavior.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using GP.Windows.UI.Interactivity;
using Hercules.App.Modules;
using Hercules.App.Modules.Editor.ViewModels;
using Hercules.Model.ExImport;
using Hercules.Model.Utils;

namespace Hercules.App.Controls
{
    public sealed class ImportGeneratorBehavior : Behavior<MenuFlyoutSubItem>
    {
        public static readonly DependencyProperty EditorViewModelProperty =
            DependencyProperty.Register("EditorViewModel", typeof(EditorViewModel), typeof(ImportGeneratorBehavior), new PropertyMetadata(null, (d, e) => ((ImportGeneratorBehavior)d).OnEditorViewModelChanged()));
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
                    foreach (IImportSource source in viewModel.ImportSources)
                    {
                        string text = ResourceManager.GetString($"ImportSource_{source.NameKey}");

                        MenuFlyoutSubItem targetItem = new MenuFlyoutSubItem { Text = text };

                        foreach (IImporter importer in viewModel.Importers)
                        {
                            text = ResourceManager.GetString($"Importer_{importer.NameKey}");

                            MenuFlyoutItem importButton = new MenuFlyoutItem();
                            importButton.Text = text;
                            importButton.CommandParameter = new ImportModel { Source = source, Importer = importer };
                            importButton.Command = viewModel.ImportCommand;

                            targetItem.Items?.Add(importButton);
                        }

                        items.Add(targetItem);
                    }
                }
            }
        }
    }
}
