using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace MindmapApp.Controls
{
    public class MindmapFlyout : Flyout
    {
        public MindmapFlyout()
        {
            Opened += (sender, e) =>
            {
                MindmapFlyoutView view = Content as MindmapFlyoutView;

                if (view != null)
                {
                    view.OnOpened();
                }
            };

            Opening += (sender, e) =>
            {
                MindmapFlyoutView view = Content as MindmapFlyoutView;

                if (view != null)
                {
                    view.OnOpening();
                }
            };

            Closed += (sender, e) =>
            {
                MindmapFlyoutView view = Content as MindmapFlyoutView;

                if (view != null)
                {
                    view.OnClosed();
                }
            };
        }
    }
}
