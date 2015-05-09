// ==========================================================================
// App.xaml.cs
// RavenMind Application
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using GalaSoft.MvvmLight.Messaging;
using RavenMind.Messages;
using System;
using System.Diagnostics;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace RavenMind
{
    public sealed partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            Suspending += App_Suspending;
        }

        protected override void OnLaunched(LaunchActivatedEventArgs args)
        {
            Window.Current.VisibilityChanged += Current_VisibilityChanged;

            if (args.PreviousExecutionState == ApplicationExecutionState.Running)
            {
                Window.Current.Activate();
            }
            else
            {
                Frame rootFrame = new Frame();

                if (!rootFrame.Navigate(typeof(MainPage)))
                {
                    throw new Exception("Failed to create initial page");
                }

                Window.Current.Content = rootFrame;
                Window.Current.Activate();
            }

            LoadMindmap(args);
        }

        private static void LoadMindmap(LaunchActivatedEventArgs args)
        {
            Guid mindmapId = Guid.Empty;

            if (Guid.TryParse(args.Arguments, out mindmapId))
            {
                Messenger.Default.Send(new OpenMindmapMessage(mindmapId));
            }
            else
            {
                Messenger.Default.Send(new OpenMindmapMessage());
            }
        }

        private void Current_VisibilityChanged(object sender, VisibilityChangedEventArgs e)
        {
            if (!e.Visible)
            {
                Messenger.Default.Send(new SaveMindmapMessage(null));
            }
        }

        private void App_Suspending(object sender, SuspendingEventArgs e)
        {
            SuspendingDeferral deferral = null;
            try
            {
                deferral = e.SuspendingOperation.GetDeferral();

                Stopwatch w = Stopwatch.StartNew();

                Messenger.Default.Send(new SaveMindmapMessage(() =>
                {
                    deferral.Complete();
                    w.Stop();
                    Debug.WriteLine("Suspended in: " + w.Elapsed);
                }));
            }
            catch
            {
                deferral.Complete();
            }
        }
    }
}
