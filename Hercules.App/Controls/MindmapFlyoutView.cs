using Hercules.Model;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Hercules.App.Controls
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

        protected MindmapFlyoutView()
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
