// ==========================================================================
// MainPage.cs
// RavenMind Application
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using RavenMind.Controls;
using RavenMind.Model;
using RavenMind.ViewModels;
using SE.Metro.UI;
using SE.Metro.UI.Interactivity;
using System;
using System.Globalization;
using System.Text;
using Windows.ApplicationModel.DataTransfer;
using Windows.ApplicationModel.Resources;
using Windows.Foundation;
using Windows.Graphics.Display;
using Windows.UI.ApplicationSettings;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media.Animation;

namespace RavenMind
{
    public sealed partial class MainPage : Page
    {
        private const double SettingsWidth = 646;
        
        private Popup settingsPopup;

        public EditorViewModel EditorViewModel
        {
            get 
            { 
                return MainGrid.DataContext as EditorViewModel; 
            }
        }

        public MindmapsViewModel MindmapsViewModel
        {
            get 
            { 
                return TopGrid.DataContext as MindmapsViewModel; 
            }
        }
        
        public MainPage()
        {
            InitializeComponent();

            GotoVisualState(); 
            ShareSourceLoad();

            Window.Current.SizeChanged += Current_SizeChanged;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            if (!MindmapsViewModel.SettingsProvider.IsTutorialShown)
            {
                PopupHandler.ShowPopupCenter<TutorialView>();

                MindmapsViewModel.SettingsProvider.IsTutorialShown = true;
            }
        }

        protected override void OnNavigatedTo(Windows.UI.Xaml.Navigation.NavigationEventArgs e)
        {
            InputPane inputPane = InputPane.GetForCurrentView();

            if (inputPane != null)
            {
                inputPane.Showing += InputPane_Showing;
                inputPane.Hiding += InputPane_Hiding;
            }

            SettingsPane settingsPane = SettingsPane.GetForCurrentView();

            if (settingsPane != null)
            {
                settingsPane.CommandsRequested += settingsPane_CommandsRequested;
            }

            DisplayProperties.OrientationChanged += DisplayProperties_OrientationChanged;
            DisplayProperties.AutoRotationPreferences = DisplayOrientations.Landscape | DisplayOrientations.LandscapeFlipped;

            base.OnNavigatedTo(e);
        }

        private void DisplayProperties_OrientationChanged(object sender)
        {
            if (DisplayProperties.CurrentOrientation == DisplayOrientations.Portrait || DisplayProperties.CurrentOrientation == DisplayOrientations.LandscapeFlipped)
            {
                DisplayProperties.AutoRotationPreferences = DisplayOrientations.Landscape | DisplayOrientations.LandscapeFlipped;
            }
        }

        protected override void OnNavigatedFrom(Windows.UI.Xaml.Navigation.NavigationEventArgs e)
        {
            InputPane inputPane = InputPane.GetForCurrentView();

            if (inputPane != null)
            {
                inputPane.Showing -= InputPane_Showing;
                inputPane.Hiding -= InputPane_Hiding;
            }

            SettingsPane settingsPane = SettingsPane.GetForCurrentView();

            if (settingsPane != null)
            {
                settingsPane.CommandsRequested -= settingsPane_CommandsRequested;
            }

            base.OnNavigatedFrom(e);
        }

        private void OnShowAbout(IUICommand command)
        {
            Window.Current.Activated += Current_Activated;

            double w = ActualWidth;
            double h = ActualHeight;

            AboutView aboutView = new AboutView();
            aboutView.Width = SettingsWidth;
            aboutView.Height = h;

            var edge = SettingsPane.Edge == SettingsEdgeLocation.Right ? EdgeTransitionLocation.Right : EdgeTransitionLocation.Left;

            settingsPopup = new Popup();
            settingsPopup.Closed += settingsPopup_Closed;
            settingsPopup.IsLightDismissEnabled = true;
            settingsPopup.Width = w;
            settingsPopup.Height = h;

            settingsPopup.ChildTransitions = new TransitionCollection();
            settingsPopup.ChildTransitions.Add(new PaneThemeTransition { Edge = edge });
            settingsPopup.Child = aboutView;
            settingsPopup.VerticalOffset = 0;
            settingsPopup.HorizontalOffset = SettingsPane.Edge == SettingsEdgeLocation.Right ? w - SettingsWidth : 0;
            settingsPopup.IsOpen = true;
        }

        private void settingsPopup_Closed(object sender, object e)
        {
            Window.Current.Activated -= Current_Activated;
        }

        private void Current_Activated(object sender, WindowActivatedEventArgs e)
        {
            if (e.WindowActivationState == CoreWindowActivationState.Deactivated)
            {
                settingsPopup.IsOpen = false;
            }
        }

        private void OnShowTutorial(IUICommand command)
        {
            PopupHandler.ShowPopupCenter<TutorialView>();
        }

        private void settingsPane_CommandsRequested(SettingsPane sender, SettingsPaneCommandsRequestedEventArgs args)
        {
            ResourceLoader resourceLoader = new ResourceLoader();

            args.Request.ApplicationCommands.Add(new SettingsCommand("ShowTutorial", resourceLoader.GetString("ShowTutorial"), OnShowTutorial));
            args.Request.ApplicationCommands.Add(new SettingsCommand("ShowAboutUs", resourceLoader.GetString("ShowAboutUs"), OnShowAbout));
        }
        
        private void InputPane_Hiding(InputPane sender, InputPaneVisibilityEventArgs args)
        {
            if (EditorViewModel.Document != null)
            {
                BottomAppBar.AnimateY(0, TimeSpan.FromSeconds(0.15));

                args.EnsuredFocusedElementInView = true;
            }
        }

        private void InputPane_Showing(InputPane sender, InputPaneVisibilityEventArgs args)
        {
            if (EditorViewModel.Document != null)
            {
                BottomAppBar.AnimateY(args.OccludedRect.Height * (-1), TimeSpan.FromSeconds(0.2));

                args.EnsuredFocusedElementInView = true;
            }
        }

        public void ShareSourceLoad()
        {
            DataTransferManager dataTransferManager = DataTransferManager.GetForCurrentView();

            dataTransferManager.DataRequested += dataTransferManager_DataRequested;
        }

        private void dataTransferManager_DataRequested(DataTransferManager sender, DataRequestedEventArgs args)
        {
            if (EditorViewModel.Document != null)
            {
                ResourceLoader resourceLoader = new ResourceLoader();

                IOutlineGenerator outlineGenerator = new HtmlOutlineGenerator();

                StringBuilder htmlBuilder = new StringBuilder();
                htmlBuilder.Append("<br/>");
                htmlBuilder.Append(outlineGenerator.GenerateOutline(EditorViewModel.Document, true, resourceLoader.GetString("NoText")));
                htmlBuilder.Append("<br />");
                htmlBuilder.Append("<br />");
                htmlBuilder.Append(resourceLoader.GetString("ShareMindmapFooter"));

                string htmlFormat = HtmlFormatHelper.CreateHtmlFormat(htmlBuilder.ToString());

                string title = string.Format(CultureInfo.CurrentCulture, resourceLoader.GetString("ShareMindmapTitle"), EditorViewModel.Document.Name);
                string descr = string.Format(CultureInfo.CurrentCulture, resourceLoader.GetString("ShareMindmapDescription"), EditorViewModel.Document.Name);

                args.Request.Data.Properties.Title = title;
                args.Request.Data.Properties.Description = descr;
                args.Request.Data.SetHtmlFormat(htmlFormat);
            }
        }

        private void Current_SizeChanged(object sender, WindowSizeChangedEventArgs e)
        {
            GotoVisualState();
        }

        private void GotoVisualState()
        {
            if (ApplicationView.Value == ApplicationViewState.Snapped)
            {
                VisualStateManager.GoToState(this, "Snapped", true);     
            }
            else
            {
                VisualStateManager.GoToState(this, "Normal", true);
            }
        }

        private async void TopGrid_Loaded(object sender, RoutedEventArgs e)
        {
            await MindmapsViewModel.LoadAsync();
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            Control senderElement = (Control)sender;

            PopupHandler.ShowPopupRightTop(new EnterNameView { DataContext = senderElement.DataContext }, new Point(-20, 110));
        }

        private void EditColorButton_Click(object sender, RoutedEventArgs e)
        {
            TopAppBar.IsOpen = false;

            if (Mindmap.Document != null && Mindmap.Document.SelectedNode != null)
            {
                PopupHandler.ShowPopupLeftBottom(new EditColorView(Mindmap), new Point(170, -90));
            }
        }

        private void EditIconButton_Click(object sender, RoutedEventArgs e)
        {
            TopAppBar.IsOpen = false;

            if (Mindmap.Document != null && Mindmap.Document.SelectedNode != null)
            {
                PopupHandler.ShowPopupLeftBottom(new EditIconView(Mindmap), new Point(270, -90));
            }
        }

        private void EditMindmapButton_Click(object sender, RoutedEventArgs e)
        {
            TopAppBar.IsOpen = false;

            if (Mindmap.Document != null)
            {
                PopupHandler.ShowPopupLeftBottom(new EditView(Mindmap), new Point(20, -90));
            }
        }

        private void RemoveButton_Invoking(object sender, CommandInvokingEventHandler e)
        {
            TextBox textBox = FocusManager.GetFocusedElement() as TextBox;

            if (textBox != null)
            {
                e.Handled = true;
            }
        }

        private void MoveLeftCommand_Invoked(object sender, RoutedEventArgs e)
        {
            if (Mindmap.Document != null)
            {
                Mindmap.Document.SelectLeftOfSelectedNode();
            }
        }

        private void MoveRightCommand_Invoked(object sender, RoutedEventArgs e)
        {
            if (Mindmap.Document != null)
            {
                Mindmap.Document.SelectRightOfSelectedNode();
            }
        }

        private void MoveTopCommand_Invoked(object sender, RoutedEventArgs e)
        {
            if (Mindmap.Document != null)
            {
                Mindmap.Document.SelectedTopOfSelectedNode();
            }
        }

        private void MoveBottomCommand_Invoked(object sender, RoutedEventArgs e)
        {
            if (Mindmap.Document != null)
            {
                Mindmap.Document.SelectedBottomOfSelectedNode();
            }
        }
    }
}
