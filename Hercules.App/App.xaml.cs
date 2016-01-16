// ==========================================================================
// App.xaml.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System;
using System.Diagnostics;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using GalaSoft.MvvmLight.Messaging;
using Hercules.App.Messages;
using Microsoft.ApplicationInsights;

namespace Hercules.App
{
    public sealed partial class App
    {
        public static readonly TelemetryClient TelemetryClient = new TelemetryClient();

        public App()
        {
            WindowsAppInitializer.InitializeAsync();

            InitializeComponent();

            Suspending += OnSuspending;
        }

        protected override void OnLaunched(LaunchActivatedEventArgs args)
        {
            ShowUI(args.Arguments);
        }

        protected override void OnFileActivated(FileActivatedEventArgs args)
        {
            ShowUI(args);

            StorageFile file = args.Files[0] as StorageFile;

            if (file != null)
            {
                Messenger.Default.Send(new OpenMindmapMessage(file));
            }
        }

        private void ShowUI(object arguments)
        {
#if DEBUG
            DebugSettings.EnableFrameRateCounter |= Debugger.IsAttached;
#endif
            Frame rootFrame = Window.Current.Content as Frame;

            if (rootFrame == null)
            {
                rootFrame = new Frame();

                rootFrame.NavigationFailed += OnNavigationFailed;

                Window.Current.Content = rootFrame;
            }

            if (rootFrame.Content == null)
            {
                rootFrame.Navigate(typeof(MainPage), arguments);
            }

            Window.Current.Activate();
        }

        private static void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception($"Failed to load Page {e.SourcePageType.FullName}");
        }

        private static void OnSuspending(object sender, SuspendingEventArgs e)
        {
            SuspendingDeferral deferral = e.SuspendingOperation.GetDeferral();

            Messenger.Default.Send(new SaveMindmapMessage(deferral.Complete));
        }
    }
}
