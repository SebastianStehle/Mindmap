// ==========================================================================
// ViewModelLocator.cs
// RavenMind Application
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using GalaSoft.MvvmLight.Messaging;
using RavenMind.Components;
using RavenMind.Model.Storing.Json;
using RavenMind.Model.Storing.Utils;
using System.Composition;
using System.Composition.Hosting;
using Windows.ApplicationModel;

namespace RavenMind.ViewModels
{
    public class ViewModelLocator
    {
        [Import]
        public EditorViewModel Editor { get; set; }

        [Import]
        public MindmapsViewModel Mindmaps { get; set; }

        public ViewModelLocator()
        {
            if (!DesignMode.DesignModeEnabled)
            {
                ContainerConfiguration configuration =
                    new ContainerConfiguration()
                        .WithPart<Messenger>()
                        .WithPart<MindmapsViewModel>()
                        .WithPart<EditorViewModel>()
                        .WithPart<JsonDocumentStore>()
                        .WithPart<ResourcesLocalizationManager>()
                        .WithPart<DefaultSettingsProvider>();

                CompositionHost host = configuration.CreateContainer();

                host.SatisfyImports(this);
            }
        }
    }
}