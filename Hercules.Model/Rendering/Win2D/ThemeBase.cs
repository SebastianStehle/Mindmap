using GP.Windows;
using GP.Windows.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hercules.Model.Rendering.Win2D
{
    public abstract class ThemeBase
    {
        private readonly List<ThemeColor> colors = new List<ThemeColor>();

        public IReadOnlyList<ThemeColor> Colors
        {
            get
            {
                return colors;
            }
        }

        protected ThemeBase()
        {
        }

        public ThemeColor FindColor(NodeBase node)
        {
            Guard.NotNull(node, nameof(node));

            return colors[node.Color];
        }

        public void AddThemeColors(params int[] newColors)
        {
            foreach (int color in newColors)
            {
                colors.Add(new ThemeColor(
                    ColorsHelper.ConvertToColor(color, 0, 0, 0),
                    ColorsHelper.ConvertToColor(color, 0, 0.2, -0.3),
                    ColorsHelper.ConvertToColor(color, 0, -0.2, 0.2)));
            }
        }

        public virtual Win2DRenderNode CreatePreviewNode()
        {
            throw new NotImplementedException();
        }

        public virtual Win2DRenderNode CreateRenderNode(NodeBase node)
        {
            throw new NotImplementedException();
        }
    }
}
