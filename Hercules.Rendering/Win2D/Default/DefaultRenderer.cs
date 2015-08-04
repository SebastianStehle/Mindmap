using Hercules.Model;

namespace Hercules.Rendering.Win2D.Default
{
    public class DefaultRenderer : ThemeRenderer
    {
        public DefaultRenderer()
        {
            AddColors(
                0xF7977A,
                0xF9AD81,
                0xFDC68A,
                0xFFF79A,
                0xC4DF9B,
                0xF26C4F,
                0xF68E55,
                0xFBAF5C,
                0xFFF467,
                0xACD372,
                0xA2D39C,
                0x82CA9D,
                0x7BCDC8,
                0x6ECFF6,
                0x7EA7D8,
                0x7CC576,
                0x3BB878,
                0x1ABBB4,
                0x00BFF3,
                0x438CCA,
                0x8493CA,
                0x8882BE,
                0xBC8DBF,
                0xF49AC2,
                0xF6989D,
                0x605CA8,
                0x855FA8,
                0xA763A8,
                0xF06EA9,
                0xF26D7D);
        }

        protected override ThemeRenderNode CreateRenderNode(NodeBase node)
        {
            if (node is RootNode)
            {
                return new DefaultRootNode(node, this);
            }
            else if (node.Parent is RootNode)
            {
                return new DefaultLevel1Node(node, this);
            }
            else
            {
                return new DefaultLevel2Node(node, this);
            }
        }
    }
}
