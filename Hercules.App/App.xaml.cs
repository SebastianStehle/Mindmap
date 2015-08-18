// ==========================================================================
// App.xaml.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================
using Microsoft.ApplicationInsights;
using System;
using System.Diagnostics;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Hercules.App.Modules.Mindmaps.Views;

namespace Hercules.App
{
    sealed partial class App
    {
        public static TelemetryClient telemetryClient = new TelemetryClient();

        public App()
        {
            telemetryClient = new TelemetryClient();

            InitializeComponent();

            Suspending += OnSuspending;
        }
        
        protected override void OnLaunched(LaunchActivatedEventArgs args)
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
                rootFrame.Navigate(typeof(MindmapsPage), args.Arguments);
            }

            Window.Current.Activate();
        }
        
        private void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception($"Failed to load Page {e.SourcePageType.FullName}");
        }

        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            SuspendingDeferral deferral = e.SuspendingOperation.GetDeferral();

            deferral.Complete();
        }
    }
}
