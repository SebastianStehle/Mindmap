using Microsoft.VisualStudio.TestPlatform.TestExecutor;
using System.Diagnostics;
using Windows.ApplicationModel.Activation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace UnitTests
{
    sealed partial class App : Application
    {
        public App()
        {
            InitializeComponent();
        }
        
        protected override void OnLaunched(LaunchActivatedEventArgs e)
        {
#if DEBUG
            DebugSettings.EnableFrameRateCounter |= Debugger.IsAttached;
#endif
            Frame rootFrame = Window.Current.Content as Frame;
            
            if (rootFrame == null)
            {
                rootFrame = new Frame();

                Window.Current.Content = rootFrame;
            }
            
            UnitTestClient.CreateDefaultUI();
            
            Window.Current.Activate();

            UnitTestClient.Run(e.Arguments);
        }
    }
}
