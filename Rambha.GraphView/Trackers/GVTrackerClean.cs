using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace Rambha.GraphView
{
    public class GVTrackerClean : GVTrackerBase
    {
        public GVTrackerClean()
        {
            Title = "Reset";
        }

        public override void OnDraw(GVGraphViewContext context, RectangleF paintRect)
        {
            base.OnDraw(context, paintRect);
            context.Graphics.DrawImage(Properties.Resources.TrackerCleanIcon, paintRect);
        }
    }
}
