using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace Rambha.GraphView
{
    public class GVTrackerDataOut: GVTrackerConnectionBase
    {
        public GVTrackerDataOut()
        {
            Title = "Data Property";
        }

        public override GVGraphConnection GetLinkType(GVGraph g)
        {
            return new GVGraphConnDataFlow(g);
        }

        public override Image GetTrackerIcon()
        {
            return Properties.Resources.DataOutIcon;
        }

        public override void OnTouchEnd(GVGraphViewContext context)
        {
            base.OnTouchEnd(context);
        }
    }
}
