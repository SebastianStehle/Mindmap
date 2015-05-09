// ==========================================================================
// ViewModelLocator.cs
// RavenMind Application
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using GalaSoft.MvvmLight.Messaging;
using RavenMind.Components;
using RavenMind.Model;
using System.Composition;
using System.Composition.Hosting;
using Windows.ApplicationModel;

namespace RavenMind.ViewModels
{
    public class ViewModelLocator
    {
        #region Properties

        [Import]
        public EditorViewModel Editor { get; set; }

        [Import]
        public MindmapsViewModel Mindmaps { get; set; }

        #endregion

        #region Methods

        public ViewModelLocator()
        {
            if (!DesignMode.DesignModeEnabled)
            {
                ContainerConfiguration configuration =
                    new ContainerConfiguration()
                        .WithPart<Messenger>()
                        .WithPart<MindmapsViewModel>()
                        .WithPart<EditorViewModel>()
                        .WithPart<DocumentStore>()
                        .WithPart<ResourcesLocalizationManager>()
                        .WithPart<DefaultSettingsProvider>();

                CompositionHost host = configuration.CreateContainer();

                host.SatisfyImports(this);
            }
        }

        #endregion
    }
}