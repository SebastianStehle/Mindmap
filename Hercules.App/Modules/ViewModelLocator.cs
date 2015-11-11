// ==========================================================================
// ViewModelLocator.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using Windows.ApplicationModel;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Views;
using GP.Windows.Mvvm;
using Hercules.App.Components;
using Hercules.App.Components.Implementations;
using Hercules.App.Modules.Editor.ViewModels;
using Hercules.App.Modules.Mindmaps.ViewModels;
using Hercules.Model.ExImport;
using Hercules.Model.ExImport.Channels.Email;
using Hercules.Model.ExImport.Channels.File;
using Hercules.Model.ExImport.Formats.Html;
using Hercules.Model.ExImport.Formats.Image;
using Hercules.Model.ExImport.Formats.Mindapp;
using Hercules.Model.ExImport.Formats.XMind;
using Hercules.Model.Storing;
using Hercules.Model.Storing.Json;
using Microsoft.Practices.Unity;

namespace Hercules.App.Modules
{
    public class ViewModelLocator
    {
        public static IUnityContainer Container { get; set; }

        [Dependency]
        public EditorViewModel Editor { get; set; }

        [Dependency]
        public MindmapsViewModel Mindmaps { get; set; }

        [Dependency]
        public IProcessManager ProcessManager { get; set; }

        public ViewModelLocator()
        {
            if (!DesignMode.DesignModeEnabled)
            {
                IUnityContainer unityContainer = new UnityContainer();

                unityContainer.RegisterType<IMessenger, Messenger>(
                    new ContainerControlledLifetimeManager());
                unityContainer.RegisterType<IDocumentStore, JsonDocumentStore>(
                    new ContainerControlledLifetimeManager());
                unityContainer.RegisterType<ISettingsProvider, DefaultSettingsProvider>(
                    new ContainerControlledLifetimeManager());
                unityContainer.RegisterType<IProcessManager, ProcessManager>(
                    new ContainerControlledLifetimeManager());
                unityContainer.RegisterType<INavigationService, NavigationService>(
                    new ContainerControlledLifetimeManager());
                unityContainer.RegisterType<IOutlineGenerator, HtmlOutlineExporter>(
                    new ContainerControlledLifetimeManager());
                unityContainer.RegisterType<IPrintService, PrintService>(
                    new ContainerControlledLifetimeManager());
                unityContainer.RegisterType<IMindmapStore, MindmapStore>(
                    new ContainerControlledLifetimeManager());
                unityContainer.RegisterType<IMessageDialogService, MessageDialogService>(
                    new ContainerControlledLifetimeManager());
                unityContainer.RegisterType<IImportSource, FileImportSource>("File",
                    new ContainerControlledLifetimeManager());
                unityContainer.RegisterType<IImporter, XMindImporter>("XMind",
                    new ContainerControlledLifetimeManager());
                unityContainer.RegisterType<IImporter, MindappImporter>("Mindapp",
                    new ContainerControlledLifetimeManager());
                unityContainer.RegisterType<IExportTarget, FileExportTarget>("File",
                    new ContainerControlledLifetimeManager());
                unityContainer.RegisterType<IExportTarget, EmailExportTarget>("Email",
                    new ContainerControlledLifetimeManager());
                unityContainer.RegisterType<IExporter, ImageExporter>("Image",
                    new ContainerControlledLifetimeManager());
                unityContainer.RegisterType<IExporter, XMindExporter>("XMind",
                    new ContainerControlledLifetimeManager());
                unityContainer.RegisterType<IExporter, MindappExporter>("Mindapp",
                    new ContainerControlledLifetimeManager());
                unityContainer.RegisterType<IExporter, HtmlOutlineExporter>("HtmlOutline",
                    new ContainerControlledLifetimeManager());
                unityContainer.RegisterType<MindmapsViewModel>(
                    new PerResolveLifetimeManager());
                unityContainer.RegisterType<EditorViewModel>(
                    new PerResolveLifetimeManager());

                unityContainer.BuildUp(this);

                Container = unityContainer;
            }
        }
    }
}