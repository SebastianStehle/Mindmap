using MindmapApp.Model;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace MindmapApp.Controls
{
    public abstract class MindmapFlyoutView : UserControl
    {
        public static readonly DependencyProperty DocumentProperty =
            DependencyProperty.Register("Document", typeof(Document), typeof(MindmapFlyoutView), new PropertyMetadata(null, new PropertyChangedCallback(OnDocumentChanged)));
        public Document Document
        {
            get { return (Document)GetValue(DocumentProperty); }
            set
            {
                SetValue(DocumentProperty, value);
            }
        }

        private static void OnDocumentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {

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
