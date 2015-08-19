using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Navigation;
using Hercules.App.Modules.Mindmaps.ViewModels;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using GP.Windows.UI;

namespace Hercules.App.Modules.Mindmaps.Views
{
    public sealed partial class MindmapsView
    {
        public MindmapsView()
        {
            InitializeComponent();

            Loaded += MindmapsPage_Loaded;
        }

        private async void MindmapsPage_Loaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            MindmapsViewModel viewModel = DataContext as MindmapsViewModel;

            if (viewModel != null)
            {
                await viewModel.LoadAsync();
            }
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            Control senderElement = (Control)sender;

            PopupHandler.ShowPopupRightTop(new EnterNameView { DataContext = senderElement.DataContext }, new Point(-80, 110));
        }

        private void Grid_Holding(object sender, Windows.UI.Xaml.Input.HoldingRoutedEventArgs e)
        {
            FrameworkElement senderElement = sender as FrameworkElement;
            
            FlyoutBase.GetAttachedFlyout(senderElement).ShowAt(senderElement);
        }

        private void Grid_PointerPressed(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {

        }

        private void Grid_RightTapped(object sender, Windows.UI.Xaml.Input.RightTappedRoutedEventArgs e)
        {
            FrameworkElement senderElement = sender as FrameworkElement;

            FlyoutBase.GetAttachedFlyout(senderElement).ShowAt(senderElement);

        }
    }
}
