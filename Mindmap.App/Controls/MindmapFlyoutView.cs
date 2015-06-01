using MindmapApp.Model;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace MindmapApp.Controls
{
    public abstract class MindmapFlyoutView : UserControl
    {
        public static readonly DependencyProperty DocumentProperty =
            DependencyProperty.Register("Document", typeof(Document), typeof(MindmapFlyoutView), new PropertyMetadata(null));
        public Document Document
        {
            get { return (Document)GetValue(DocumentProperty); }
            set { SetValue(DocumentProperty, value); }
        }

        public static readonly DependencyProperty ThemeProperty =
            DependencyProperty.Register("Theme", typeof(ThemeBase), typeof(MindmapFlyoutView), new PropertyMetadata(null));
        public ThemeBase Theme
        {
            get { return (ThemeBase)GetValue(ThemeProperty); }
            set { SetValue(ThemeProperty, value); }
        }
       

        public MindmapFlyoutView()
        {
        }

        public virtual void OnOpening()
        {
        }

        public virtual void OnOpened()
        {
        }

        public virtual void OnClosed()
        {
        }
    }
}
