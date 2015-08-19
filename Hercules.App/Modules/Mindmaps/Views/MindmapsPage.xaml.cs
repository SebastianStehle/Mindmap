using Windows.UI.Xaml.Navigation;
using Hercules.App.Modules.Mindmaps.ViewModels;

namespace Hercules.App.Modules.Mindmaps.Views
{
    public sealed partial class MindmapsPage
    {
        public MindmapsPage()
        {
            InitializeComponent();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            MindmapsViewModel viewModel = (MindmapsViewModel)DataContext;

            await viewModel.LoadAsync();
        }
    }
}
