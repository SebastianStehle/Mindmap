﻿// ==========================================================================
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
using Microsoft.HockeyApp;

namespace Hercules.App
{
    public sealed partial class App
    {
        public App()
        {
            HockeyClient.Current.Configure("9753e6d94abc4e3bae685877c81a5d52");

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

            var file = args.Files[0] as StorageFile;

            if (file != null)
            {
                Messenger.Default.Send(new OpenMessage(file));
            }
        }

        private void ShowUI(object arguments)
        {
#if DEBUG
            DebugSettings.EnableFrameRateCounter |= Debugger.IsAttached;
#endif
            var rootFrame = Window.Current.Content as Frame;

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
            var deferral = e.SuspendingOperation.GetDeferral();

            Messenger.Default.Send(new SaveMessage(deferral.Complete));
        }
    }
}
