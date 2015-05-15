using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;

namespace RavenMind.Model.Layouting.Default
{
    public class DropZone
    {
        private readonly AttachTarget target;
        private readonly Rect bounds;

        public AttachTarget Target
        {
            get
            {
                return target;
            }
        }

        public Rect Bounds
        {
            get
            {
                return bounds;
            }
        }

        public DropZone(AttachTarget target, Rect bounds)
        {
            this.target = target;
            this.bounds = bounds;
        }
    }
}
