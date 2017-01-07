using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace Rambha.GraphView
{
    public class GVTrackerControlOut : GVTrackerConnectionBase
    {
        public GVTrackerControlOut()
        {
            Title = "Control Out";
        }

        public override GVGraphConnection GetLinkType(GVGraph g)
        {
            return new GVGraphConnControlFlow(g);
        }

        public override Image GetTrackerIcon()
        {
            return Properties.Resources.TrackerOutIcon;
        }

        public override void OnTouchEnd(GVGraphViewContext context)
        {
            // source and target have to be of appropriate types
            //
            if (context.TrackedObject.AcceptsTracker(this))
            {
            }
            else
            {
                context.TrackedObject = null;
            }

            base.OnTouchEnd(context);
        }
    }
}
